package de.jandev.ls4apiserver.service;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.exception.ApplicationExceptionCode;
import de.jandev.ls4apiserver.model.champselect.*;
import de.jandev.ls4apiserver.model.collection.champion.Champion;
import de.jandev.ls4apiserver.model.collection.skin.Skin;
import de.jandev.ls4apiserver.model.event.GameLobbyFinalEvent;
import de.jandev.ls4apiserver.model.event.GameLobbyUpdateEvent;
import de.jandev.ls4apiserver.model.lobby.LobbyType;
import de.jandev.ls4apiserver.model.user.User;
import de.jandev.ls4apiserver.service.utility.LobbyPhaseWorkerTask;
import de.jandev.ls4apiserver.service.utility.LogMessage;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.slf4j.helpers.MessageFormatter;
import org.springframework.context.ApplicationEventPublisher;
import org.springframework.http.HttpStatus;
import org.springframework.scheduling.TaskScheduler;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Service;

import java.time.Instant;
import java.util.*;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.CopyOnWriteArrayList;
import java.util.concurrent.ScheduledFuture;

@Service
public class ChampselectService {

    private static final Logger LOGGER = LoggerFactory.getLogger(ChampselectService.class);
    private final List<GameLobby> gameLobbies = new CopyOnWriteArrayList<>();
    private final UserCollectionService userCollectionService;
    private final TaskScheduler taskScheduler;
    private final ApplicationEventPublisher applicationEventPublisher;
    private final Map<GameLobby, ScheduledFuture<?>> tasks = new ConcurrentHashMap<>();

    public ChampselectService(UserCollectionService userCollectionService, TaskScheduler taskScheduler, ApplicationEventPublisher applicationEventPublisher) {
        this.userCollectionService = userCollectionService;
        this.taskScheduler = taskScheduler;
        this.applicationEventPublisher = applicationEventPublisher;
    }

    @Scheduled(initialDelay = 5000, fixedDelay = 1000)
    private void decreaseTradeTimer() {
        for (GameLobby gameLobby : gameLobbies) {
            List<ChampionTrade> toRemove = new ArrayList<>();

            for (ChampionTrade championTrade : gameLobby.getTrades()) {
                championTrade.setTimer(championTrade.getTimer() - 1);

                if (championTrade.getTimer() <= 0) {
                    toRemove.add(championTrade);
                }
            }

            gameLobby.getTrades().removeAll(toRemove);
        }
    }

    // Must be called AFTER all players are initialized into GameLobby instance
    public void setupLobby(GameLobby gameLobby) {
        var lobbyType = gameLobby.getLobbyType();

        if (lobbyType == LobbyType.CUSTOMMAP_RIFT_DRAFT || lobbyType == LobbyType.TWISTED_TREELINE_DRAFT || lobbyType == LobbyType.OLD_TWISTED_TREELINE_DRAFT) {
            gameLobby.setMaxBansTeam1(lobbyType.getMaxBans());
            gameLobby.setMaxBansTeam2(lobbyType.getMaxBans());

            if (!gameLobby.getTeam1().isEmpty()) {
                gameLobby.setLobbyPhase(LobbyPhase.BAN_TEAM1);
                gameLobby.getTeam1().get(0).setCanBan(true);
            } else {
                gameLobby.setLobbyPhase(LobbyPhase.BAN_TEAM2);
                gameLobby.setMaxBansTeam1(0);
                gameLobby.getTeam2().get(0).setCanBan(true);
            }

            if (gameLobby.getTeam2().isEmpty()) {
                gameLobby.setMaxBansTeam2(0);
            }
        } else if (lobbyType == LobbyType.CUSTOMMAP_RIFT_BLIND || lobbyType == LobbyType.TWISTED_TREELINE_BLIND || lobbyType == LobbyType.OLD_TWISTED_TREELINE_BLIND || lobbyType == LobbyType.CRYSTAL_SCAR_BLIND) {
            gameLobby.setLobbyPhase(LobbyPhase.PICK_BLIND);
            gameLobby.setMaxBansTeam1(0);
            gameLobby.setMaxBansTeam2(0);
        } else if (lobbyType == LobbyType.ARAM_BLIND) {
            gameLobby.setLobbyPhase(LobbyPhase.PRE_START_ARAM);
            gameLobby.setMaxBansTeam1(0);
            gameLobby.setMaxBansTeam2(0);

            generateRandomChampions(gameLobby);
        }

        updatePlayersWhoCanPickAndBan(gameLobby);

        gameLobby.setTimer(gameLobby.getLobbyPhase().getDuration());
    }

    public void push(GameLobby gameLobby) {
        gameLobbies.add(gameLobby);

        setEnemyVisibilityAndUpdate(gameLobby);
        tasks.put(gameLobby, taskScheduler.schedule(new LobbyPhaseWorkerTask(applicationEventPublisher, this, gameLobby), Instant.now().plusSeconds(gameLobby.getLobbyPhase().getDuration())));

        LOGGER.info(LogMessage.CHAMPSELECT_ADDED, gameLobby.getUuid(), gameLobby.getLobbyPhase());
    }

    public void pull(GameLobby gameLobby) {
        if (tasks.containsKey(gameLobby)) {
            tasks.get(gameLobby).cancel(true);
            tasks.remove(gameLobby);
        }
        gameLobbies.remove(gameLobby);

        LOGGER.info(LogMessage.CHAMPSELECT_REMOVED, gameLobby.getUuid(), gameLobby.getLobbyPhase());
    }

    private void startNextBanPhase(GameLobby gameLobby) {
        if (
                gameLobby.getBansTeam1().size() == gameLobby.getMaxBansTeam1() &&
                        gameLobby.getBansTeam2().size() == gameLobby.getMaxBansTeam2()
        ) {
            startPickPhase(gameLobby);
        } else {
            if (gameLobby.getLobbyPhase() == LobbyPhase.BAN_TEAM1) {
                if (!gameLobby.getTeam2().isEmpty()) {
                    gameLobby.setLobbyPhase(LobbyPhase.BAN_TEAM2);
                } else {
                    gameLobby.setLobbyPhase(LobbyPhase.BAN_TEAM1);
                }
            } else {
                if (!gameLobby.getTeam1().isEmpty()) {
                    gameLobby.setLobbyPhase(LobbyPhase.BAN_TEAM1);
                } else {
                    gameLobby.setLobbyPhase(LobbyPhase.BAN_TEAM2);
                }
            }
        }
        gameLobby.setTimer(gameLobby.getLobbyPhase().getDuration());
    }

    private void startPickPhase(GameLobby gameLobby) {
        // start pick phase
        // purpose of checking this is if it is a 1 person lobby on team 2
        if (!gameLobby.getTeam1().isEmpty()) {
            gameLobby.setLobbyPhase(LobbyPhase.PICK_TEAM1);
            gameLobby.getTeam1().get(0).setCanBan(false);
        } else {
            gameLobby.setLobbyPhase(LobbyPhase.PICK_TEAM2);
            gameLobby.getTeam2().get(0).setCanBan(false);
        }
    }

    private void startNextPickPhase(GameLobby gameLobby) {
        if (gameLobby.getAmountPickedTeam1() == gameLobby.getTeam1().size() &&
                gameLobby.getAmountPickedTeam2() == gameLobby.getTeam2().size()
        ) {
            gameLobby.setLobbyPhase(LobbyPhase.PRE_START);
        } else if (gameLobby.getLobbyPhase() == LobbyPhase.PICK_TEAM1) {
            if (!gameLobby.getTeam2().isEmpty()) {
                gameLobby.setLobbyPhase(LobbyPhase.PICK_TEAM2);
            } else {
                gameLobby.setLobbyPhase(LobbyPhase.PICK_TEAM1);
            }
        } else {
            if (!gameLobby.getTeam1().isEmpty()) {
                gameLobby.setLobbyPhase(LobbyPhase.PICK_TEAM1);
            } else {
                gameLobby.setLobbyPhase(LobbyPhase.PICK_TEAM2);
            }
        }

        gameLobby.setTimer(gameLobby.getLobbyPhase().getDuration());
    }

    public void startNextPhase(GameLobby gameLobby) {
        if (tasks.containsKey(gameLobby)) {
            tasks.get(gameLobby).cancel(true);
            tasks.remove(gameLobby);
        }

        if (gameLobby.getLobbyPhase() != LobbyPhase.PRE_START && gameLobby.getLobbyPhase() != LobbyPhase.PRE_START_ARAM) {
            if (gameLobby.getLobbyPhase() == LobbyPhase.BAN_TEAM1 || gameLobby.getLobbyPhase() == LobbyPhase.BAN_TEAM2) {
                // check and update banning phase
                startNextBanPhase(gameLobby);
            } else if (gameLobby.getLobbyPhase() == LobbyPhase.PICK_TEAM1 || gameLobby.getLobbyPhase() == LobbyPhase.PICK_TEAM2) {
                // check and update picking phase
                startNextPickPhase(gameLobby);
            } else if (gameLobby.getLobbyPhase() == LobbyPhase.PICK_BLIND) {
                gameLobby.setLobbyPhase(LobbyPhase.PRE_START);
                gameLobby.setTimer(LobbyPhase.PRE_START.getDuration());
            }

            updatePlayersWhoCanPickAndBan(gameLobby);
            setEnemyVisibilityAndUpdate(gameLobby);
            tasks.put(gameLobby, taskScheduler.schedule(new LobbyPhaseWorkerTask(applicationEventPublisher, this, gameLobby), Instant.now().plusSeconds(gameLobby.getLobbyPhase().getDuration())));
        } else {
            applicationEventPublisher.publishEvent(new GameLobbyFinalEvent(this, gameLobby, false));
        }
    }

    private void setEnemyVisibilityAndUpdate(GameLobby gameLobby) {
        if (gameLobby.getLobbyType() == LobbyType.CUSTOMMAP_RIFT_DRAFT || gameLobby.getLobbyType() == LobbyType.TWISTED_TREELINE_DRAFT || gameLobby.getLobbyType() == LobbyType.OLD_TWISTED_TREELINE_DRAFT) {
            for (LobbyUser lobbyUser : gameLobby.getAllUsers()) {
                lobbyUser.setVisibleToEnemy(lobbyUser.isCanBan() || lobbyUser.isCanLockIn() || lobbyUser.isLockedIn());
            }
        } else {
            for (LobbyUser lobbyUser : gameLobby.getAllUsers()) {
                lobbyUser.setVisibleToEnemy(false); // Champion not visible to enemy team on blind pick
            }
        }

        applicationEventPublisher.publishEvent(new GameLobbyUpdateEvent(this, gameLobby));
    }

    public void banLockChampion(GameLobby gameLobby, LobbyUser lobbyUser, String championId) throws ApplicationException {
        if (gameLobby.getLobbyType() != LobbyType.CUSTOMMAP_RIFT_DRAFT && gameLobby.getLobbyType() != LobbyType.TWISTED_TREELINE_DRAFT && gameLobby.getLobbyType() != LobbyType.OLD_TWISTED_TREELINE_DRAFT) {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.CHAMPSELECT_MESSAGE_FORBIDDEN, LogMessage.CHAMPSELECT_MESSAGE_FORBIDDEN);
        }

        var champion = userCollectionService.getChampionByIdWithoutSpellsAndSkins(championId);

        if (lobbyUser.isCanBan()) {
            if (!gameLobby.getAllBans().contains(champion)) {
                if (lobbyUser.getTeam() == LobbyTeam.TEAM1) {
                    gameLobby.getBansTeam1().add(champion);
                } else {
                    gameLobby.getBansTeam2().add(champion);
                }

                lobbyUser.setCanBan(false);
                lobbyUser.setSelectedChampion(null);
                unselectPickedChampion(gameLobby, champion, lobbyUser, true);

                // Reset already preselected champion from other players
                startNextPhase(gameLobby);
            }
        } else {
            throw new ApplicationException(HttpStatus.FORBIDDEN, ApplicationExceptionCode.CHAMPSELECT_BAN_FORBIDDEN, LogMessage.CHAMPSELECT_BAN_FORBIDDEN);
        }
    }

    public void lockChampion(GameLobby gameLobby, LobbyUser lobbyUser, String championId, boolean cycleAfterLock) throws
            ApplicationException {
        var champion = userCollectionService.getChampionByIdWithoutSpellsAndSkins(championId);

        if (lobbyUser.isCanLockIn() && !gameLobby.getAllBans().contains(champion)) {
            var bothTeams = true;

            if (gameLobby.getLobbyType() == LobbyType.CUSTOMMAP_RIFT_DRAFT || gameLobby.getLobbyType() == LobbyType.TWISTED_TREELINE_DRAFT || gameLobby.getLobbyType() == LobbyType.OLD_TWISTED_TREELINE_DRAFT) {
                if (gameLobby.getAllPickedChampions().contains(champion)) {
                    throw new ApplicationException(HttpStatus.FORBIDDEN, ApplicationExceptionCode.CHAMPSELECT_PICK_FORBIDDEN, LogMessage.CHAMPSELECT_PICK_FORBIDDEN);
                }
            } else {
                // For blind pick only unselect for own team
                bothTeams = false;

                if (gameLobby.getAllPickedChampionsTeam(lobbyUser.getTeam()).contains(champion)) {
                    throw new ApplicationException(HttpStatus.FORBIDDEN, ApplicationExceptionCode.CHAMPSELECT_PICK_FORBIDDEN, LogMessage.CHAMPSELECT_PICK_FORBIDDEN);
                }
            }

            lobbyUser.setSelectedChampion(champion);
            lobbyUser.setCanLockIn(false);
            lobbyUser.setLockedIn(true);
            unselectPickedChampion(gameLobby, champion, lobbyUser, bothTeams);

            if (gameLobby.getAllUsers().stream().noneMatch(LobbyUser::isCanLockIn) && cycleAfterLock) {
                startNextPhase(gameLobby);
            }
        } else {
            throw new ApplicationException(HttpStatus.FORBIDDEN, ApplicationExceptionCode.CHAMPSELECT_PICK_FORBIDDEN, LogMessage.CHAMPSELECT_PICK_FORBIDDEN);
        }
    }

    private void unselectPickedChampion(GameLobby gameLobby, Champion champion, LobbyUser picker, boolean bothTeams) {
        if (bothTeams) {
            for (LobbyUser member : gameLobby.getAllUsers()) {
                if (member.getSelectedChampion() != null
                        && champion.equals(member.getSelectedChampion())
                        && !picker.getUser().equals(member.getUser())) {
                    member.setSelectedChampion(null);
                }
            }
        } else {
            for (LobbyUser member : gameLobby.getTeam(picker.getTeam())) {
                if (member.getSelectedChampion() != null
                        && champion.equals(member.getSelectedChampion())
                        && !picker.getUser().equals(member.getUser())) {
                    member.setSelectedChampion(null);
                }
            }
        }
    }

    // Users are always allowed to change their spells
    public void selectSpell(GameLobby gameLobby, LobbyUser lobbyUser, String summonerSpellString, boolean one) throws
            ApplicationException {
        Optional<SummonerSpell> summonerSpell = Arrays.stream(SummonerSpell.values()).filter(c -> c.name().equalsIgnoreCase(summonerSpellString)).findFirst();

        if (summonerSpell.isPresent() && checkSpellForGameMode(gameLobby, summonerSpell.get())) {
            if (lobbyUser.getSpell1() == summonerSpell.get() || lobbyUser.getSpell2() == summonerSpell.get()) {
                SummonerSpell tempSpell1 = lobbyUser.getSpell1();
                lobbyUser.setSpell1(lobbyUser.getSpell2());
                lobbyUser.setSpell2(tempSpell1);
            } else {
                if (one) {
                    lobbyUser.setSpell1(summonerSpell.get());
                } else {
                    lobbyUser.setSpell2(summonerSpell.get());
                }
            }
        } else {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.CHAMPSELECT_INVALID_SPELL, MessageFormatter.format(LogMessage.CHAMPSELECT_INVALID_SPELL, summonerSpellString).getMessage());
        }
    }

    private boolean checkSpellForGameMode(GameLobby gameLobby, SummonerSpell summonerSpell) {
        if (gameLobby.getLobbyType() == LobbyType.ARAM_BLIND) {
            return summonerSpell != SummonerSpell.SUMMONER_ODIN_GARRISON
                    && summonerSpell != SummonerSpell.SUMMONER_CLAIRVOYANCE
                    && summonerSpell != SummonerSpell.SUMMONER_TELEPORT
                    && summonerSpell != SummonerSpell.SUMMONER_SMITE;
        } else {
            return summonerSpell != SummonerSpell.SUMMONER_ODIN_GARRISON;
        }
    }


    public void selectSkin(LobbyUser lobbyUser, String skinName) throws ApplicationException {
        var userSkins = userCollectionService.getOwnedSkinsFromUser(lobbyUser.getUser().getUserName());

        Optional<Skin> skin = userSkins.stream().filter(c -> c.getName().equals(skinName)).findFirst();

        if (skin.isPresent()) {
            lobbyUser.setSelectedSkin(skin.get());
        } else {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.CHAMPSELECT_INVALID_SKIN, MessageFormatter.format(LogMessage.CHAMPSELECT_INVALID_SKIN, skinName).getMessage());
        }
    }

    public Optional<LobbyUser> getLobbyUserFromGameLobbyByUsername(GameLobby gameLobby, String userName) {
        return gameLobby.getAllUsers().stream().filter(c -> c.getUser().getUserName().equals(userName)).findFirst();
    }

    public Optional<GameLobby> getGameLobbyByUserName(String userName) {
        return gameLobbies.stream().filter(c -> c.getAllUsers().stream().anyMatch(t -> t.getUser().getUserName().equals(userName))).findFirst();
    }

    public List<User> getDodger(GameLobby gameLobby) {
        List<User> dodgers = new ArrayList<>();

        // for each user, if that user can lock in or can ban, they are a dodger
        for (LobbyUser user : gameLobby.getAllUsers()) {
            if (user.isCanLockIn() || user.isCanBan()) {
                dodgers.add(user.getUser());
            }
        }

        return dodgers;
    }

    public void updatePlayersWhoCanPickAndBan(GameLobby gameLobby) {
        var team1Size = gameLobby.getTeam1().size();
        var team2Size = gameLobby.getTeam2().size();
        var amountLeftToPickTeam1 = team1Size - gameLobby.getAmountPickedTeam1();
        var amountLeftToPickTeam2 = team2Size - gameLobby.getAmountPickedTeam2();

        switch (gameLobby.getLobbyPhase()) {
            case BAN_TEAM1:
                gameLobby.getTeam1().get(0).setCanBan(true);
                break;
            case BAN_TEAM2:
                gameLobby.getTeam2().get(0).setCanBan(true);
                break;
            case PICK_TEAM1:
                if (gameLobby.getAmountPickedTeam1() == 0) {
                    gameLobby.getTeam1().get(0).setCanLockIn(true);
                } else {
                    findUsersWhoCanLockIn(gameLobby.getTeam1(), team1Size, team2Size, amountLeftToPickTeam1, gameLobby.getAmountPickedTeam1(), gameLobby.getAmountPickedTeam2());
                }
                break;
            case PICK_TEAM2:
                findUsersWhoCanLockIn(gameLobby.getTeam2(), team2Size, team1Size, amountLeftToPickTeam2, gameLobby.getAmountPickedTeam2(), gameLobby.getAmountPickedTeam1());
                break;
            case PICK_BLIND:
                for (LobbyUser lobbyUser : gameLobby.getAllUsers()) {
                    lobbyUser.setCanLockIn(!lobbyUser.isLockedIn());
                }
                break;
            case PRE_START_ARAM:
                for (LobbyUser lobbyUser : gameLobby.getAllUsers()) {
                    lobbyUser.setCanLockIn(false);
                }
                break;
            default:
                break;
        }
    }

    private void findUsersWhoCanLockIn(List<LobbyUser> currentTeam, int sizeOfCurrentTeam, int sizeOfOtherTeam,
                                       int amountLeftToPickCurrentTeam, int amountPickedCurrentTeam, int amountPickedOtherTeam) {
        if (amountPickedOtherTeam == sizeOfOtherTeam) {
            for (int i = amountPickedCurrentTeam; i < sizeOfCurrentTeam; i++) {
                currentTeam.get(i).setCanLockIn(true);
            }
        } else if (amountLeftToPickCurrentTeam < 2) {
            currentTeam.get(amountPickedCurrentTeam).setCanLockIn(true);
        } else {
            // allow next 2 team members to pick
            currentTeam.get(amountPickedCurrentTeam).setCanLockIn(true);
            currentTeam.get(amountPickedCurrentTeam + 1).setCanLockIn(true);
        }
    }

    public void handleTradeRequest(GameLobby gameLobby, LobbyUser initiator, String targetSummonerName) throws ApplicationException {
        if (isNotInTradeGameMode(gameLobby)) {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.CHAMPSELECT_MESSAGE_FORBIDDEN, LogMessage.CHAMPSELECT_MESSAGE_FORBIDDEN);
        }

        Optional<LobbyUser> target = gameLobby.getTeam(initiator.getTeam()).stream().filter(c -> c.getUser().getSummonerName().equals(targetSummonerName)).findFirst();

        if (target.isPresent()
                && (gameLobby.getLobbyPhase() == LobbyPhase.PRE_START || gameLobby.getLobbyPhase() == LobbyPhase.PRE_START_ARAM)
                && isNotTrading(gameLobby, initiator) && isNotTrading(gameLobby, target.get())) {
            gameLobby.getTrades().add(new ChampionTrade(initiator, target.get(), initiator.getTeam(), 10));
        } else {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.CHAMPSELECT_TRADE_NOT_POSSIBLE, LogMessage.CHAMPSELECT_TRADE_NOT_POSSIBLE);
        }
    }

    public void handleTradeRequestReply(GameLobby gameLobby, LobbyUser target, String initiatorSummonerName, boolean accepted) throws ApplicationException {
        if (isNotInTradeGameMode(gameLobby)) {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.CHAMPSELECT_MESSAGE_FORBIDDEN, LogMessage.CHAMPSELECT_MESSAGE_FORBIDDEN);
        }

        Optional<LobbyUser> initiator = gameLobby.getTeam(target.getTeam()).stream().filter(c -> c.getUser().getSummonerName().equals(initiatorSummonerName)).findFirst();

        if (initiator.isPresent()
                && (gameLobby.getLobbyPhase() == LobbyPhase.PRE_START || gameLobby.getLobbyPhase() == LobbyPhase.PRE_START_ARAM)
                && isTradingWith(gameLobby, target, initiator.get())) {
            if (accepted) {
                var initiatorChampionBackup = userCollectionService.getChampionByIdWithoutSpellsAndSkins(initiator.get().getSelectedChampion().getId());

                initiator.get().setSelectedChampion(target.getSelectedChampion());
                target.setSelectedChampion(initiatorChampionBackup);

                initiator.get().setSelectedSkin(null);
                target.setSelectedSkin(null);
            }

            gameLobby.getTrades().removeIf(c -> c.getInitiator().equals(initiator.get()) && c.getTarget().equals(target));
        } else {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.CHAMPSELECT_TRADE_NOT_POSSIBLE, LogMessage.CHAMPSELECT_TRADE_NOT_POSSIBLE);
        }
    }

    private boolean isNotInTradeGameMode(GameLobby gameLobby) {
        return gameLobby.getLobbyType() != LobbyType.CUSTOMMAP_RIFT_DRAFT && gameLobby.getLobbyType() != LobbyType.ARAM_BLIND && gameLobby.getLobbyType() != LobbyType.TWISTED_TREELINE_DRAFT && gameLobby.getLobbyType() != LobbyType.OLD_TWISTED_TREELINE_DRAFT;
    }

    private boolean isNotTrading(GameLobby gameLobby, LobbyUser lobbyUser) {
        return gameLobby.getTrades().stream().noneMatch(c -> c.getInitiator().equals(lobbyUser) || c.getTarget().equals(lobbyUser));
    }

    private boolean isTradingWith(GameLobby gameLobby, LobbyUser lobbyUser, LobbyUser lobbyUser2) {
        return gameLobby.getTrades().stream().anyMatch(c -> c.getInitiator().equals(lobbyUser) && c.getTarget().equals(lobbyUser2)
                || c.getInitiator().equals(lobbyUser2) && c.getTarget().equals(lobbyUser));
    }

    private void generateRandomChampions(GameLobby gameLobby) {
        var champions = userCollectionService.getAllChampionsWithoutSpellsAndSkins();
        for (LobbyUser lobbyUser : gameLobby.getAllUsers()) {
            Collections.shuffle(champions);

            try {
                lobbyUser.setCanLockIn(true);
                lockChampion(gameLobby, lobbyUser, champions.get(0).getId(), false);
            } catch (ApplicationException e) {
                LOGGER.error("Error locking champion in ARAM. This should never happen.", e);
            }

            champions.remove(0);
        }
    }

}

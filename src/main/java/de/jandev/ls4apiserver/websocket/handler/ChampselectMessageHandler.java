package de.jandev.ls4apiserver.websocket.handler;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.exception.ApplicationExceptionCode;
import de.jandev.ls4apiserver.model.champselect.GameLobby;
import de.jandev.ls4apiserver.model.champselect.LobbyPhase;
import de.jandev.ls4apiserver.model.champselect.LobbyTeam;
import de.jandev.ls4apiserver.model.champselect.LobbyUser;
import de.jandev.ls4apiserver.model.event.GameLobbyFinalEvent;
import de.jandev.ls4apiserver.model.event.GameLobbyUpdateEvent;
import de.jandev.ls4apiserver.model.lobby.Lobby;
import de.jandev.ls4apiserver.model.lobby.LobbyType;
import de.jandev.ls4apiserver.model.user.User;
import de.jandev.ls4apiserver.model.websocket.MessageType;
import de.jandev.ls4apiserver.model.websocket.SocketMessage;
import de.jandev.ls4apiserver.model.websocket.champselect.*;
import de.jandev.ls4apiserver.model.websocket.chat.ChatMessageOut;
import de.jandev.ls4apiserver.model.websocket.lobby.LobbyMessageOut;
import de.jandev.ls4apiserver.service.*;
import de.jandev.ls4apiserver.service.utility.LogMessage;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.event.EventListener;
import org.springframework.http.HttpStatus;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.stereotype.Component;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Optional;
import java.util.stream.Collectors;

@Component
public class ChampselectMessageHandler {

    private static final Logger LOGGER = LoggerFactory.getLogger(ChampselectMessageHandler.class);
    private static final String QUEUE_CHAMPSELECT = "/queue/champselect";
    private static final String QUEUE_LOBBY = "/queue/lobby";
    private final SimpMessagingTemplate template;
    private final LobbyService lobbyService;
    private final MatchmakingService matchmakingService;
    private final ChampselectService champselectService;
    private final GameManagerService gameManagerService;
    private final UserCollectionService userCollectionService;

    @Value("${timer.dodge}")
    private int dodgeTime;

    public ChampselectMessageHandler(SimpMessagingTemplate template, LobbyService lobbyService,
                                     MatchmakingService matchmakingService, ChampselectService champselectService,
                                     GameManagerService gameManagerService, UserCollectionService userCollectionService) {
        this.template = template;
        this.lobbyService = lobbyService;
        this.matchmakingService = matchmakingService;
        this.champselectService = champselectService;
        this.gameManagerService = gameManagerService;
        this.userCollectionService = userCollectionService;
    }

    public void handleLostConnection(User user) {
        Optional<GameLobby> gameLobby = champselectService.getGameLobbyByUserName(user.getUserName());
        gameLobby.ifPresent(lobby -> handleDodge(lobby, Collections.singletonList(user)));
    }

    public void handleDodge(GameLobby gameLobby, List<User> dodgers) {
        List<Lobby> lobbies = new ArrayList<>();

        champselectService.pull(gameLobby);

        var isCustom = false;

        for (LobbyUser member : gameLobby.getAllUsers()) {
            template.convertAndSendToUser(member.getUser().getUserName(), QUEUE_CHAMPSELECT + gameLobby.getUuid(),
                    new SocketMessage(null, null, null, MessageType.CHAMPSELECT_ABANDON, LocalDateTime.now()));

            Optional<Lobby> lobby = lobbyService.getLobbyByUser(member.getUser());
            if (lobby.isPresent() && !lobbies.contains(lobby.get())) {
                if (lobbies.isEmpty()) {
                    isCustom = lobby.get().isCustom();
                }
                lobbies.add(lobby.get());
            }
        }

        if (!isCustom) {
            addDodgeTimer(gameLobby, dodgers);
        }

        for (Lobby lobby : lobbies) {
            // We can use Collections.disjoint here, but that function is even worse in my opinion
            if (lobby.getMembers().stream().anyMatch(c -> dodgers.stream().map(User::getUserName).collect(Collectors.toList()).contains(c.getUserName()))) {
                lobby.setInQueue(false);

                for (User lobbyMember : lobby.getMembers()) {
                    template.convertAndSendToUser(lobbyMember.getUserName(), QUEUE_LOBBY + lobby.getUuid(),
                            new SocketMessage(new LobbyMessageOut(lobby), null, null, MessageType.LOBBY_UPDATE, LocalDateTime.now()));
                }
            } else {
                matchmakingService.repushInternal(lobby);
            }
        }
    }

    private void addDodgeTimer(GameLobby gameLobby, List<User> dodgers) {
        for (User user : dodgers) {
            matchmakingService.getDodgeTimers().put(user.getUuid(), dodgeTime);
            LOGGER.info(LogMessage.CHAMPSELECT_DODGED, user.getUserName(), gameLobby.getUuid(), dodgeTime);
        }
    }

    public void handleChat(GameLobby gameLobby, LobbyUser lobbyUser, String message) {
        for (LobbyUser member : gameLobby.getTeam(lobbyUser.getTeam())) {
            template.convertAndSendToUser(member.getUser().getUserName(), QUEUE_CHAMPSELECT + gameLobby.getUuid(),
                    new SocketMessage(new ChatMessageOut(lobbyUser.getUser().getSummonerName(), null, message, LocalDateTime.now()), null, null, MessageType.CHAMPSELECT_CHAT, LocalDateTime.now()));
        }
    }

    public void handleSelectChampion(GameLobby gameLobby, LobbyUser lobbyUser, String championId) throws ApplicationException {
        var champion = userCollectionService.getChampionByIdWithoutSpellsAndSkins(championId);

        if (!lobbyUser.isLockedIn() && !gameLobby.getAllPickedChampions().contains(champion) && !gameLobby.getAllBans().contains(champion)) {
            lobbyUser.setSelectedChampion(champion);
            sendAllUpdate(gameLobby);
        }
    }

    public void handleBanSelectChampion(GameLobby gameLobby, LobbyUser lobbyUser, String championId) throws ApplicationException {
        if (gameLobby.getLobbyType() != LobbyType.CUSTOMMAP_RIFT_DRAFT && gameLobby.getLobbyType() != LobbyType.TWISTED_TREELINE_DRAFT && gameLobby.getLobbyType() != LobbyType.OLD_TWISTED_TREELINE_DRAFT ) {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.CHAMPSELECT_MESSAGE_FORBIDDEN, LogMessage.CHAMPSELECT_MESSAGE_FORBIDDEN);
        }

        var champion = userCollectionService.getChampionByIdWithoutSpellsAndSkins(championId);

        if (((gameLobby.getLobbyPhase() == LobbyPhase.BAN_TEAM1 && gameLobby.getTeam1().get(0).equals(lobbyUser))
                || (gameLobby.getLobbyPhase() == LobbyPhase.BAN_TEAM2 && gameLobby.getTeam2().get(0).equals(lobbyUser)))
                && !gameLobby.getAllPickedChampions().contains(champion) && !gameLobby.getAllBans().contains(champion)) {
            lobbyUser.setSelectedChampion(champion);
            sendAllUpdate(gameLobby);
        } else {
            throw new ApplicationException(HttpStatus.FORBIDDEN, ApplicationExceptionCode.CHAMPSELECT_BAN_FORBIDDEN, LogMessage.CHAMPSELECT_BAN_FORBIDDEN);
        }
    }

    public void handleBanLockChampion(GameLobby gameLobby, LobbyUser lobbyUser, String championId) throws ApplicationException {
        champselectService.banLockChampion(gameLobby, lobbyUser, championId);
        sendAllUpdate(gameLobby);
    }

    public void handleLockChampion(GameLobby gameLobby, LobbyUser lobbyUser, String championId) throws ApplicationException {
        champselectService.lockChampion(gameLobby, lobbyUser, championId, true);
        sendAllUpdate(gameLobby);
    }

    @EventListener
    public void handleGameLobbyUpdateEvent(GameLobbyUpdateEvent gameLobbyUpdateEvent) {
        sendAllUpdate(gameLobbyUpdateEvent.getGameLobby());
    }

    @EventListener
    public void handleGameLobbyFinalEvent(GameLobbyFinalEvent gameLobbyFinalEvent) {
        if (gameLobbyFinalEvent.isKill()) {
            handleDodge(gameLobbyFinalEvent.getGameLobby(), champselectService.getDodger(gameLobbyFinalEvent.getGameLobby()));
        } else {
            // We need to pull the game lobby, otherwise weird stuff can happen. For example: A member of the pulled game lobby dodges another game lobby, the other lobbies will be repushed twice,
            // because we use user search in handleLostConnection. Or if we accidentally send unsubscribe.
            // This also protects from changing any value in GameLobby object after the game started. (e.g Trading)
            champselectService.pull(gameLobbyFinalEvent.getGameLobby());

            List<GameStartOut> gameStartOuts = gameManagerService.start(gameLobbyFinalEvent.getGameLobby());

            for (var i = 0; i < gameLobbyFinalEvent.getGameLobby().getAllUsers().size(); i++) {
                LobbyUser member = gameLobbyFinalEvent.getGameLobby().getAllUsers().get(i);

                if (gameStartOuts.size() == gameLobbyFinalEvent.getGameLobby().getAllUsers().size()) {
                    template.convertAndSendToUser(member.getUser().getUserName(), QUEUE_CHAMPSELECT + gameLobbyFinalEvent.getGameLobby().getUuid(),
                            new SocketMessage(gameStartOuts.get(i), null, null, MessageType.CHAMPSELECT_GAME_START, LocalDateTime.now()));
                } else {
                    template.convertAndSendToUser(member.getUser().getUserName(), QUEUE_CHAMPSELECT + gameLobbyFinalEvent.getGameLobby().getUuid(),
                            new SocketMessage(null, null, null, MessageType.CHAMPSELECT_GAME_START, LocalDateTime.now()));
                }
            }

            // We don't need to remove the lobby from lobbyService as the client is already unsubscribing. We shouldn't rely on the client, but this is an intended behavior as of now in case we want to keep the lobby in the future.
            // If we're sending an abandon here then we are not resetting the inQueue here nor are we sending a lobby update, thus the lobby members all need to press abandon queue by themselves.
        }
    }

    public void handleSelectSpell(GameLobby gameLobby, LobbyUser lobbyUser, String summonerSpellString, boolean one) throws ApplicationException {
        champselectService.selectSpell(gameLobby, lobbyUser, summonerSpellString, one);
        sendAllUpdate(gameLobby);
    }

    public void handleSelectSkin(GameLobby gameLobby, LobbyUser lobbyUser, String skinName) throws ApplicationException {
        champselectService.selectSkin(lobbyUser, skinName);
        sendAllUpdate(gameLobby);
    }

    public void sendAllUpdate(GameLobby gameLobby) {
        var gameLobbyOut1 = new GameLobbyOut(gameLobby);
        var gameLobbyOut2 = new GameLobbyOut(gameLobby);

        handleVisibility(gameLobby, gameLobby.getTeam1(), gameLobbyOut1, gameLobbyOut2);
        handleVisibility(gameLobby, gameLobby.getTeam2(), gameLobbyOut2, gameLobbyOut1);

        gameLobbyOut1.setTradesTeam(gameLobby.getTrades().stream().filter(c -> c.getLobbyTeam() == LobbyTeam.TEAM1).map(ChampionTradeOut::new).collect(Collectors.toList()));
        gameLobbyOut2.setTradesTeam(gameLobby.getTrades().stream().filter(c -> c.getLobbyTeam() == LobbyTeam.TEAM2).map(ChampionTradeOut::new).collect(Collectors.toList()));

        for (LobbyUser member : gameLobby.getTeam1()) {
            template.convertAndSendToUser(member.getUser().getUserName(), QUEUE_CHAMPSELECT + gameLobby.getUuid(),
                    new SocketMessage(gameLobbyOut1, null, null, MessageType.CHAMPSELECT_UPDATE, LocalDateTime.now()));
        }

        for (LobbyUser member : gameLobby.getTeam2()) {
            template.convertAndSendToUser(member.getUser().getUserName(), QUEUE_CHAMPSELECT + gameLobby.getUuid(),
                    new SocketMessage(gameLobbyOut2, null, null, MessageType.CHAMPSELECT_UPDATE, LocalDateTime.now()));
        }
    }

    private void handleVisibility(GameLobby gameLobby, List<LobbyUser> currentTeam, GameLobbyOut currentTeamGameLobbyOut, GameLobbyOut otherTeamGameLobbyOut) {
        for (LobbyUser lobbyUser : currentTeam) {
            currentTeamGameLobbyOut.getTeam().add(new LobbyUserOut(lobbyUser));

            if (lobbyUser.isVisibleToEnemy() && (gameLobby.getLobbyType() == LobbyType.CUSTOMMAP_RIFT_DRAFT || gameLobby.getLobbyType() == LobbyType.TWISTED_TREELINE_DRAFT || gameLobby.getLobbyType() == LobbyType.OLD_TWISTED_TREELINE_DRAFT)) {
                otherTeamGameLobbyOut.getEnemyTeam().add(new LobbyUserEnemyOut(lobbyUser, true));
            } else {
                otherTeamGameLobbyOut.getEnemyTeam().add(new LobbyUserEnemyOut(lobbyUser, false));
            }
        }
    }

    public Optional<GameLobby> getGameLobbyByUserName(String userName) {
        return champselectService.getGameLobbyByUserName(userName);
    }

    public Optional<LobbyUser> getLobbyUserFromGameLobbyByUsername(GameLobby gameLobby, String userName) {
        return champselectService.getLobbyUserFromGameLobbyByUsername(gameLobby, userName);
    }

    public void handleTradeRequest(GameLobby gameLobby, LobbyUser initiator, String targetSummonerName) throws ApplicationException {
        champselectService.handleTradeRequest(gameLobby, initiator, targetSummonerName);
        sendAllUpdate(gameLobby);
    }

    public void handleTradeRequestReply(GameLobby gameLobby, LobbyUser initiator, String targetSummonerName, boolean accepted) throws ApplicationException {
        champselectService.handleTradeRequestReply(gameLobby, initiator, targetSummonerName, accepted);
        sendAllUpdate(gameLobby);
    }
}

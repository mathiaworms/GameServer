package de.jandev.ls4apiserver.service;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.exception.ApplicationExceptionCode;
import de.jandev.ls4apiserver.model.champselect.PreGameLobby;
import de.jandev.ls4apiserver.model.event.QueuePopEvent;
import de.jandev.ls4apiserver.model.lobby.Lobby;
import de.jandev.ls4apiserver.model.lobby.LobbyType;
import de.jandev.ls4apiserver.model.user.User;
import de.jandev.ls4apiserver.service.utility.LogMessage;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.slf4j.helpers.MessageFormatter;
import org.springframework.context.ApplicationEventPublisher;
import org.springframework.http.HttpStatus;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.*;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.CopyOnWriteArrayList;

@Service
public class MatchmakingService {

    private static final Logger LOGGER = LoggerFactory.getLogger(MatchmakingService.class);
    private final List<Lobby> queue = new CopyOnWriteArrayList<>();
    private final List<Lobby> queueBlocked = new CopyOnWriteArrayList<>();
    private final Map<String, Integer> dodgeTimers = new ConcurrentHashMap<>();
    private final ApplicationEventPublisher applicationEventPublisher;

    public MatchmakingService(ApplicationEventPublisher applicationEventPublisher) {
        this.applicationEventPublisher = applicationEventPublisher;
    }

    public List<Lobby> getQueueBlocked() {
        return queueBlocked;
    }

    public List<Lobby> getQueue() {
        return queue;
    }

    public Map<String, Integer> getDodgeTimers() {
        return dodgeTimers;
    }

    @Scheduled(initialDelay = 5000, fixedDelay = 1000)
    public void decreaseDodgeTimers() {
        dodgeTimers.values().removeIf(c -> c <= 0);
        dodgeTimers.forEach((k, v) -> dodgeTimers.merge(k, 1, (a, b) -> a - b));
    }

    // Please kill me :)
    @Scheduled(initialDelay = 5000, fixedDelay = 5000)
    public void checkQueue() {
        Map<LobbyType, List<Lobby>> filteredByType = new EnumMap<>(LobbyType.class);
        List<PreGameLobby> preGameLobbies = new ArrayList<>();

        // Filter by lobby type
        for (Lobby lobby : queue) {
            if (filteredByType.containsKey(lobby.getLobbyType())) {
                filteredByType.get(lobby.getLobbyType()).add(lobby);
            } else {
                List<Lobby> lobbies = new ArrayList<>(); // No need to keep order here
                lobbies.add(lobby);
                filteredByType.put(lobby.getLobbyType(), lobbies);
            }
        }

        // Get all game lobbies for each lobby type
        filteredByType.forEach((lobbyType, allLobbiesFromType) -> preGameLobbies.addAll(createPreGameLobbies(getMatchableLobbies(allLobbiesFromType, lobbyType), lobbyType)));

        // Start game lobbies
        startPreGameLobbies(preGameLobbies);
    }

    public void push(Lobby lobby) throws ApplicationException {
        if (queue.contains(lobby)) {
            LOGGER.info(LogMessage.MATCHMAKING_ALREADY_IN_QUEUE, lobby.getUuid());
            throw new ApplicationException(HttpStatus.CONFLICT, ApplicationExceptionCode.MATCHMAKING_ALREADY_IN_QUEUE, MessageFormatter.format(LogMessage.MATCHMAKING_ALREADY_IN_QUEUE, lobby.getUuid()).getMessage());
        } else if (queueBlocked.contains(lobby)) {
            LOGGER.info(LogMessage.MATCHMAKING_BLOCKED_JOIN, lobby.getUuid());
            throw new ApplicationException(HttpStatus.FORBIDDEN, ApplicationExceptionCode.MATCHMAKING_BLOCKED_JOIN, MessageFormatter.format(LogMessage.MATCHMAKING_BLOCKED_JOIN, lobby.getUuid()).getMessage());
        }

        Map<String, String> dodgers = checkDodgeTimer(lobby);

        if (!dodgers.isEmpty()) {
            LOGGER.info(LogMessage.MATCHMAKING_BLOCKED_JOIN_DODGE, lobby.getUuid(), dodgers);

            throw new ApplicationException(HttpStatus.FORBIDDEN, ApplicationExceptionCode.MATCHMAKING_BLOCKED_JOIN_DODGE, dodgers.toString());
        }

        lobby.setJoinedQueueTime(LocalDateTime.now());
        queue.add(lobby);
        LOGGER.info(LogMessage.MATCHMAKING_JOINED_QUEUE, lobby.getUuid(), lobby.getLobbyType());

        lobby.setInQueue(true);
    }

    private Map<String, String> checkDodgeTimer(Lobby lobby) {
        Map<String, String> dodgers = new HashMap<>();

        for (User user : lobby.getMembers()) {
            Integer dodgeTime = dodgeTimers.get(user.getUuid());

            if (dodgeTime != null) {
                int minutes = dodgeTime / 60;
                dodgeTime = dodgeTime - (minutes * 60);
                int seconds = dodgeTime;

                dodgers.put(user.getSummonerName(), String.format("%d:%02d", minutes, seconds));
            }
        }
        return dodgers;
    }

    public void repushInternal(Lobby lobby) {
        // This will only be called upon a lobby rejoining due to another lobby not accepting the pop event or leaving the champ select.
        // Thus we don't need to check for dodge timers.
        if (queueBlocked.contains(lobby)) {
            LOGGER.info(LogMessage.MATCHMAKING_BLOCKED_JOIN_DEBUG, lobby.getUuid());
        }

        if (!queue.contains(lobby) && !queueBlocked.contains(lobby)) {
            queue.add(lobby);
            lobby.setInQueue(true);
            LOGGER.info(LogMessage.MATCHMAKING_REJOINED_QUEUE, lobby.getUuid(), lobby.getLobbyType());
        }
    }

    public void pullInternal(Lobby lobby) {
        if (queueBlocked.contains(lobby)) {
            LOGGER.info(LogMessage.MATCHMAKING_BLOCKED_LEAVE_DEBUG, lobby.getUuid());
        }

        if (queue.contains(lobby) && !queueBlocked.contains(lobby)) {
            queue.remove(lobby);
            lobby.setInQueue(false);
            LOGGER.info(LogMessage.MATCHMAKING_LEFT_QUEUE, lobby.getUuid(), lobby.getLobbyType());
        }
    }

    public void pull(Lobby lobby) throws ApplicationException {
        if (!queue.contains(lobby)) {
            LOGGER.info(LogMessage.MATCHMAKING_NOT_IN_QUEUE, lobby.getUuid());
            throw new ApplicationException(HttpStatus.CONFLICT, ApplicationExceptionCode.MATCHMAKING_NOT_IN_QUEUE, MessageFormatter.format(LogMessage.MATCHMAKING_NOT_IN_QUEUE, lobby.getUuid()).getMessage());
        } else if (queueBlocked.contains(lobby)) {
            LOGGER.info(LogMessage.MATCHMAKING_BLOCKED_LEAVE, lobby.getUuid());
            throw new ApplicationException(HttpStatus.CONFLICT, ApplicationExceptionCode.MATCHMAKING_BLOCKED_LEAVE, MessageFormatter.format(LogMessage.MATCHMAKING_BLOCKED_LEAVE, lobby.getUuid()).getMessage());
        }

        queue.remove(lobby);
        lobby.setInQueue(false);
        LOGGER.info(LogMessage.MATCHMAKING_LEFT_QUEUE, lobby.getUuid(), lobby.getLobbyType());
    }

    private void startPreGameLobbies(List<PreGameLobby> gameLobbies) {
        for (PreGameLobby preGameLobby : gameLobbies) {
            LOGGER.info(LogMessage.MATCHMAKING_MATCHED_LOBBY, preGameLobby.getLobbyType(), preGameLobby.getTeam1(), preGameLobby.getTeam2());

            queueBlocked.addAll(preGameLobby.getAllLobbies());
            queue.removeAll(preGameLobby.getAllLobbies());

            applicationEventPublisher.publishEvent(new QueuePopEvent(this, preGameLobby));
        }
    }

    private List<List<Lobby>> getMatchableLobbies(List<Lobby> lobbies, LobbyType lobbyType) {
        sortLobbies(lobbies);

        List<List<Lobby>> matchedLobbies = new ArrayList<>();

        // While we still have lobbies to go through
        while (!(lobbies.isEmpty())) {
            List<Lobby> team = new ArrayList<>();

            findLobbiesForTeam(lobbies, lobbyType, team);

            // Is there still a match?
            if (team.stream().mapToInt(c -> c.getMembers().size()).sum() != lobbyType.getTeamSize()) {
                return matchedLobbies;
            }

            lobbies.removeAll(team);
            matchedLobbies.add(team);
        }

        // If all lobbies could be matched
        return matchedLobbies;
    }

    private void findLobbiesForTeam(List<Lobby> lobbies, LobbyType lobbyType, List<Lobby> team) {
        var counter = 0;
        // While the team is not full
        while (team.stream().mapToInt(c -> c.getMembers().size()).sum() != lobbyType.getTeamSize()) {
            // Delete the old try
            team.clear();

            for (var i = counter; i < lobbies.size(); i++) { // We only want to add the highest member sizes [7, 6, 4, 4, 2, 1, 1, 1] -> [7, 1] [4, 4] [6, 2] not [6, 1, 1]
                // Check if this lobby fits
                int teamSize = team.stream().mapToInt(c -> c.getMembers().size()).sum();
                if (teamSize + lobbies.get(i).getMembers().size() <= lobbyType.getTeamSize()) {
                    team.add(lobbies.get(i));

                    // If full after addition no need to continue the loop
                    if (teamSize == lobbyType.getTeamSize()) {
                        break;
                    }
                }
            }

            if (counter >= lobbies.size() - 1) {
                break;
            }

            counter++;
        }
    }

    private void sortLobbies(List<Lobby> lobbies) {
        // Sort the list, biggest lobbies first, longest searching lobby first
        // For elo, we'll check if the "partner" number (e.g 4, 1, 1) has a appropriate elo (soft elo),
        // if not check if there is another matching number with a better elo, otherwise check for maximum elo difference (hard elo), if there is nothing found, it's not matchable
        // If we then matched a team, we check if there's another team in that threshold. Same process, first soft, then increase linearly depending on queue time
        Comparator<Lobby> lobbyComparatorSize = Comparator.comparing(lobby -> lobby.getMembers().size());
        Comparator<Lobby> lobbyComparatorSizeAndTime = lobbyComparatorSize.thenComparing(Lobby::getJoinedQueueTime, Comparator.reverseOrder());
        lobbies.sort(lobbyComparatorSizeAndTime.reversed());
    }

    private List<PreGameLobby> createPreGameLobbies(List<List<Lobby>> matchedLobbies, LobbyType lobbyType) {
        List<PreGameLobby> preGameLobbies = new ArrayList<>();

        for (var i = 0; i < matchedLobbies.size(); i += 2) {
            if ((i + 2) <= matchedLobbies.size()) {
                var preGameLobby = new PreGameLobby();
                preGameLobby.setLobbyType(lobbyType);

                preGameLobby.setTeam1(matchedLobbies.get(i));
                preGameLobby.setTeam2(matchedLobbies.get(i + 1));

                preGameLobbies.add(preGameLobby);
            }
        }

        return preGameLobbies;
    }
}

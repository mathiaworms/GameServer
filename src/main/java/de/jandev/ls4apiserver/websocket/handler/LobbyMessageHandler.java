package de.jandev.ls4apiserver.websocket.handler;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.exception.ApplicationExceptionCode;
import de.jandev.ls4apiserver.model.champselect.GameLobby;
import de.jandev.ls4apiserver.model.champselect.LobbyTeam;
import de.jandev.ls4apiserver.model.champselect.LobbyUser;
import de.jandev.ls4apiserver.model.champselect.PreGameLobby;
import de.jandev.ls4apiserver.model.event.LobbyRemoveEvent;
import de.jandev.ls4apiserver.model.event.QueuePopEvent;
import de.jandev.ls4apiserver.model.lobby.Lobby;
import de.jandev.ls4apiserver.model.user.User;
import de.jandev.ls4apiserver.model.user.dto.UserPublicOut;
import de.jandev.ls4apiserver.model.websocket.MessageType;
import de.jandev.ls4apiserver.model.websocket.SocketMessage;
import de.jandev.ls4apiserver.model.websocket.champselect.GameLobbyOut;
import de.jandev.ls4apiserver.model.websocket.champselect.LobbyUserEnemyOut;
import de.jandev.ls4apiserver.model.websocket.champselect.LobbyUserOut;
import de.jandev.ls4apiserver.model.websocket.chat.ChatMessageOut;
import de.jandev.ls4apiserver.model.websocket.lobby.LobbyInviteOut;
import de.jandev.ls4apiserver.model.websocket.lobby.LobbyMessageOut;
import de.jandev.ls4apiserver.model.websocket.lobby.LobbyTypeIn;
import de.jandev.ls4apiserver.service.ChampselectService;
import de.jandev.ls4apiserver.service.LobbyService;
import de.jandev.ls4apiserver.service.MatchmakingService;
import de.jandev.ls4apiserver.service.utility.LogMessage;
import de.jandev.ls4apiserver.service.utility.QueuePopEndTask;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.slf4j.helpers.MessageFormatter;
import org.springframework.context.event.EventListener;
import org.springframework.http.HttpStatus;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.scheduling.TaskScheduler;
import org.springframework.stereotype.Component;

import java.time.Instant;
import java.time.LocalDateTime;
import java.util.Collections;
import java.util.List;
import java.util.Optional;
import java.util.concurrent.CopyOnWriteArrayList;

@Component
public class LobbyMessageHandler {

    private static final Logger LOGGER = LoggerFactory.getLogger(LobbyMessageHandler.class);
    private static final String QUEUE_LOBBY = "/queue/lobby";
    private static final String QUEUE_SYSTEM = "/queue/system";
    private final List<PreGameLobby> preGameLobbies = new CopyOnWriteArrayList<>();
    private final SimpMessagingTemplate template;
    private final LobbyService lobbyService;
    private final MatchmakingService matchmakingService;
    private final ChampselectService champselectService;
    private final TaskScheduler taskScheduler;

    public LobbyMessageHandler(SimpMessagingTemplate template, LobbyService lobbyService, MatchmakingService matchmakingService, ChampselectService champselectService, TaskScheduler taskScheduler) {
        this.template = template;
        this.lobbyService = lobbyService;
        this.matchmakingService = matchmakingService;
        this.champselectService = champselectService;
        this.taskScheduler = taskScheduler;
    }

    public void initLobby(String lobbyUuid, User user) throws ApplicationException {
        Optional<Lobby> lobby = lobbyService.getLobby(lobbyUuid);

        if (lobby.isPresent() && lobby.get().getMembers().contains(user)) {
            template.convertAndSendToUser(user.getUserName(), QUEUE_LOBBY + lobbyUuid,
                    new SocketMessage(new LobbyMessageOut(lobby.get()), null, null, MessageType.LOBBY_UPDATE, LocalDateTime.now()));
        } else {
            LOGGER.info(LogMessage.LOBBY_UNAUTHORIZED, user.getUserName(), lobbyUuid);
            throw new ApplicationException(HttpStatus.FORBIDDEN, ApplicationExceptionCode.LOBBY_UNAUTHORIZED, MessageFormatter.format(LogMessage.LOBBY_UNAUTHORIZED, user.getSummonerName(), lobbyUuid).getMessage());
        }
    }

    public void handleLobbyCreate(String messageId, User user, LobbyTypeIn lobbyTypeIn) throws ApplicationException {
        var lobby = lobbyService.createLobby(user, lobbyTypeIn);

        template.convertAndSendToUser(user.getUserName(), QUEUE_SYSTEM,
                new SocketMessage(new LobbyMessageOut(lobby), messageId, null, MessageType.LOBBY_CREATE, LocalDateTime.now()));
    }

    public void handleLobbyInvite(Lobby lobby, User user, User target) throws ApplicationException {
        var updatedLobby = lobbyService.inviteMember(lobby, user.getUserName(), target);

        template.convertAndSendToUser(target.getUserName(), QUEUE_SYSTEM,
                new SocketMessage(new LobbyInviteOut(updatedLobby.getUuid(), new UserPublicOut(user)), null, null, MessageType.LOBBY_INVITE, LocalDateTime.now()));

        sendLobbyUpdate(updatedLobby);
    }

    public void handleLobbyInviteAccept(String messageId, String lobbyUuid, User user) throws ApplicationException {
        Optional<Lobby> lobbyOld = lobbyService.getLobbyByUser(user);
        Optional<Lobby> lobbyNew = lobbyService.getLobby(lobbyUuid);

        if (lobbyNew.isPresent() && (matchmakingService.getQueue().contains(lobbyNew.get()) || matchmakingService.getQueueBlocked().contains(lobbyNew.get()))) {
            throw new ApplicationException(HttpStatus.CONFLICT, ApplicationExceptionCode.LOBBY_IN_QUEUE_CANNOT_BE_JOINED, LogMessage.LOBBY_IN_QUEUE_CANNOT_BE_JOINED);
        }

        var lobby = lobbyService.acceptInvite(lobbyUuid, user);

        if (lobbyOld.isPresent()) {
            var lobbyOldNew = lobbyService.removeMemberInternal(lobbyOld.get(), user);

            sendLobbyUpdate(lobbyOldNew);
        }

        template.convertAndSendToUser(user.getUserName(), QUEUE_SYSTEM,
                new SocketMessage(new LobbyMessageOut(lobby), messageId, null, MessageType.LOBBY_ACCEPT, LocalDateTime.now()));

        sendLobbyUpdate(lobby);
    }

    public void handleLobbyInviteDeny(String lobbyUuid, User user) throws ApplicationException {
        var lobby = lobbyService.denyInvite(lobbyUuid, user);

        sendLobbyUpdate(lobby);
    }

    public void handleLobbyChangeLobbyType(Lobby lobby, User user, LobbyTypeIn lobbyTypeIn) throws ApplicationException {
        var updatedLobby = lobbyService.changeLobbyType(lobby, user, lobbyTypeIn);

        if (matchmakingService.getQueue().contains(lobby)) {
            matchmakingService.pullInternal(lobby);
        } else if (matchmakingService.getQueueBlocked().contains(lobby)) {
            lobby.setAlteredDuringAccept(true);
        }

        sendLobbyUpdate(updatedLobby);
    }

    public void handleLobbyKick(Lobby lobby, User user, User target) throws ApplicationException {
        var updatedLobby = lobbyService.kickMember(lobby, user, target);

        template.convertAndSendToUser(target.getUserName(), QUEUE_LOBBY + updatedLobby.getUuid(),
                new SocketMessage(null, null, null, MessageType.LOBBY_KICK, LocalDateTime.now()));

        if (matchmakingService.getQueue().contains(lobby)) {
            matchmakingService.pullInternal(lobby);
        } else if (matchmakingService.getQueueBlocked().contains(lobby)) {
            lobby.setAlteredDuringAccept(true);
        }

        sendLobbyUpdate(updatedLobby);
    }

    public void handleLobbyChat(Lobby lobby, User user, String message) {
        for (User member : lobby.getMembers()) {
            template.convertAndSendToUser(member.getUserName(), QUEUE_LOBBY + lobby.getUuid(),
                    new SocketMessage(new ChatMessageOut(user.getSummonerName(), null, message, LocalDateTime.now()), null, null, MessageType.LOBBY_CHAT, LocalDateTime.now()));
        }
    }

    public void handleLostConnection(User user) {
        Optional<Lobby> lobby = lobbyService.getLobbyByUser(user);
        Optional<Lobby> lobbyInvited = lobbyService.getInvitedLobbyByUser(user);

        if (lobby.isPresent()) {
            var newLobby = lobbyService.removeMemberInternal(lobby.get(), user);

            if (matchmakingService.getQueue().contains(lobby.get())) {
                matchmakingService.pullInternal(lobby.get());
            } else if (matchmakingService.getQueueBlocked().contains(lobby.get())) {
                lobby.get().setAlteredDuringAccept(true);
            }

            sendLobbyUpdate(newLobby);
        }

        if (lobbyInvited.isPresent()) {
            var newLobby = lobbyService.removeInviteInternal(lobbyInvited.get(), user);

            sendLobbyUpdate(newLobby);
        }
    }

    public void handleLobbyLeave(Lobby lobby, User user) throws ApplicationException {
        var updatedLobby = lobbyService.removeMember(lobby, user);

        if (matchmakingService.getQueue().contains(lobby)) {
            matchmakingService.pullInternal(lobby);
        } else if (matchmakingService.getQueueBlocked().contains(lobby)) {
            lobby.setAlteredDuringAccept(true);
        }

        sendLobbyUpdate(updatedLobby);
    }

    public void handleLobbyMatchmakingStart(Lobby lobby, User user) throws ApplicationException {
        lobbyService.isOwnerOrThrowException(user, lobby);

        if (lobby.getTeam1().size() > lobby.getLobbyType().getTeamSize() || lobby.getTeam2().size() > lobby.getLobbyType().getTeamSize()) {
            for (User member : lobby.getMembers()) {
                template.convertAndSendToUser(member.getUserName(), QUEUE_LOBBY + lobby.getUuid(),
                        new SocketMessage(new ChatMessageOut("SYSTEM", null, "Cannot start matchmaking because one of the teams is too big. Maximum size is: " + lobby.getLobbyType().getTeamSize() + ".", LocalDateTime.now()), null, null, MessageType.LOBBY_CHAT, LocalDateTime.now()));
            }
            return;
        }

        if (lobby.isCustom()) {
            createCustomGame(lobby);
        } else {
            matchmakingService.push(lobby);
        }

        sendLobbyUpdate(lobby);
    }

    public void handleLobbyMatchmakingStop(Lobby lobby) throws ApplicationException {
        matchmakingService.pull(lobby);

        sendLobbyUpdate(lobby);
    }

    public void handleLobbySwitchTeam(Lobby lobby, User user) throws ApplicationException {
        if (lobby.isCustom()) {

            boolean callback = lobbyService.switchTeam(lobby, user);

            if (callback) {
                sendLobbyUpdate(lobby);
            }
        } else {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.LOBBY_SWITCH_IMPOSSIBLE, LogMessage.LOBBY_SWITCH_IMPOSSIBLE);
        }
    }

    public void handleLobbyUserMatchmakingReply(Lobby lobby, User user, boolean accepted) throws ApplicationException {
        var found = false;
        for (PreGameLobby preGameLobby : preGameLobbies) {

            // If user didn't accept before already
            if (preGameLobby.getAllLobbies().contains(lobby)
                    && !preGameLobby.getLobbyAmountAccepted().get(lobby).contains(user)
                    && !preGameLobby.getLobbyAmountDenied().get(lobby).contains(user)) {
                if (accepted) {
                    preGameLobby.getLobbyAmountAccepted().get(lobby).add(user);
                    preGameLobby.getLobbyPopUpdate().setPending(preGameLobby.getLobbyPopUpdate().getPending() - 1);
                    preGameLobby.getLobbyPopUpdate().setAccepted(preGameLobby.getLobbyPopUpdate().getAccepted() + 1);
                } else {
                    preGameLobby.getLobbyAmountDenied().get(lobby).add(user);
                    preGameLobby.getLobbyPopUpdate().setPending(preGameLobby.getLobbyPopUpdate().getPending() - 1);
                    preGameLobby.getLobbyPopUpdate().setAccepted(preGameLobby.getLobbyPopUpdate().getDenied() + 1);
                }

                for (User member : lobby.getMembers()) {
                    template.convertAndSendToUser(member.getUserName(), QUEUE_LOBBY + lobby.getUuid(),
                            new SocketMessage(preGameLobby.getLobbyPopUpdate(), null, null, MessageType.LOBBY_MATCH_FOUND_UPDATE, LocalDateTime.now()));
                }

                found = true;
            }
        }

        if (!found) {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.LOBBY_MATCHMAKING_ACCEPT_IMPOSSIBLE, LogMessage.LOBBY_MATCHMAKING_ACCEPT_IMPOSSIBLE);
        }
    }

    @EventListener
    public void handleQueuePopEvent(QueuePopEvent queuePopEvent) {
        preGameLobbies.add(queuePopEvent.getPreGameLobby());

        queuePopEvent.getPreGameLobby().getLobbyPopUpdate().setPending(queuePopEvent.getPreGameLobby().getAllLobbies().stream().mapToInt(c -> c.getMembers().size()).sum());

        for (Lobby lobby : queuePopEvent.getPreGameLobby().getAllLobbies()) {
            queuePopEvent.getPreGameLobby().getLobbyAmountAccepted().put(lobby, new CopyOnWriteArrayList<>());
            queuePopEvent.getPreGameLobby().getLobbyAmountDenied().put(lobby, new CopyOnWriteArrayList<>());

            for (User member : lobby.getMembers()) {
                template.convertAndSendToUser(member.getUserName(), QUEUE_LOBBY + lobby.getUuid(),
                        new SocketMessage(null, null, null, MessageType.LOBBY_MATCH_FOUND, LocalDateTime.now()));
            }
        }

        taskScheduler.schedule(new QueuePopEndTask(queuePopEvent.getPreGameLobby(), this), Instant.now().plusSeconds(15));
    }

    @EventListener
    public void handleLobbyRemoveEvent(LobbyRemoveEvent lobbyRemoveEvent) {
        for (User user : lobbyRemoveEvent.getLobby().getInvited().keySet()) {
            template.convertAndSendToUser(user.getUserName(), QUEUE_SYSTEM,
                    new SocketMessage(new LobbyInviteOut(lobbyRemoveEvent.getLobby().getUuid(), new UserPublicOut(user)), null, null, MessageType.LOBBY_INVITE_REVOKE, LocalDateTime.now()));
        }
    }

    public void handleQueuePopEndCallback(PreGameLobby preGameLobby) {
        preGameLobbies.remove(preGameLobby);

        // The other lobbies will just be added back to the queue, they do not receive a notification, because the client knows that the queue pop time is over
        List<Lobby> notAccepted = preGameLobby.getLobbiesNotAccepted();

        if (notAccepted.isEmpty()) {
            for (Lobby lobby : preGameLobby.getAllLobbies()) {
                // Unblock lobbies from starting a new search or abandoning the search
                matchmakingService.getQueueBlocked().remove(lobby);
            }

            createGame(preGameLobby);
        } else {
            for (Lobby lobby : preGameLobby.getAllLobbies()) {
                matchmakingService.getQueueBlocked().remove(lobby);

                if (notAccepted.contains(lobby)) {
                    LOGGER.info(LogMessage.LOBBY_NOT_ACCEPTED_DEBUG, lobby.getUuid(), lobby.getMembers().size(), preGameLobby.getLobbyAmountAccepted().get(lobby).size());
                    lobby.setInQueue(false);

                    sendLobbyUpdate(lobby);
                } else {
                    LOGGER.info(LogMessage.LOBBY_REPUSH_DEBUG, lobby.getUuid());
                    matchmakingService.repushInternal(lobby);
                }

                // Reset altered
                lobby.setAlteredDuringAccept(false);
            }
        }
    }

    private void createCustomGame(Lobby lobby) {
        var preGameLobby = new PreGameLobby();
        preGameLobby.setLobbyType(lobby.getLobbyType());

        // A bit of a workaround but not that compute intensive.
        var team1 = new Lobby();
        var team2 = new Lobby();

        for (User user : lobby.getMembers()) {
            if (user.getLobbyTeam() == LobbyTeam.TEAM1) {
                team1.getMembers().add(user);
            } else {
                team2.getMembers().add(user);
            }
        }

        team1.setUuid(lobby.getUuid());
        team2.setUuid(lobby.getUuid());

        preGameLobby.setTeam1(Collections.singletonList(team1));
        preGameLobby.setTeam2(Collections.singletonList(team2));

        createGame(preGameLobby);
    }

    private void createGame(PreGameLobby preGameLobby) {
        var gameLobby = new GameLobby();
        gameLobby.setLobbyType(preGameLobby.getLobbyType());

        for (Lobby lobby : preGameLobby.getTeam1()) {
            for (User user : lobby.getMembers()) {
                var lobbyUser = new LobbyUser(user);
                lobbyUser.setTeam(LobbyTeam.TEAM1);
                gameLobby.getTeam1().add(lobbyUser);
            }
        }

        for (Lobby lobby : preGameLobby.getTeam2()) {
            for (User user : lobby.getMembers()) {
                var lobbyUser = new LobbyUser(user);
                lobbyUser.setTeam(LobbyTeam.TEAM2);
                gameLobby.getTeam2().add(lobbyUser);
            }
        }

        champselectService.setupLobby(gameLobby);

        champselectService.push(gameLobby);

        var team1Out = new GameLobbyOut(gameLobby);
        var team2Out = new GameLobbyOut(gameLobby);

        for (LobbyUser lobbyUser : gameLobby.getTeam1()) {
            team1Out.getTeam().add(new LobbyUserOut(lobbyUser));
            team2Out.getEnemyTeam().add(new LobbyUserEnemyOut(lobbyUser, false));
        }

        for (LobbyUser lobbyUser : gameLobby.getTeam2()) {
            team2Out.getTeam().add(new LobbyUserOut(lobbyUser));
            team1Out.getEnemyTeam().add(new LobbyUserEnemyOut(lobbyUser, false));
        }

        for (Lobby lobby : preGameLobby.getTeam1()) {
            for (User user : lobby.getMembers()) {
                template.convertAndSendToUser(user.getUserName(), QUEUE_LOBBY + lobby.getUuid(),
                        new SocketMessage(team1Out, null, null, MessageType.LOBBY_CHAMPSELECT_SUBSCRIBE, LocalDateTime.now()));
            }
        }

        for (Lobby lobby : preGameLobby.getTeam2()) {
            for (User user : lobby.getMembers()) {
                template.convertAndSendToUser(user.getUserName(), QUEUE_LOBBY + lobby.getUuid(),
                        new SocketMessage(team2Out, null, null, MessageType.LOBBY_CHAMPSELECT_SUBSCRIBE, LocalDateTime.now()));
            }
        }
    }

    private void sendLobbyUpdate(Lobby lobby) {
        for (User member : lobby.getMembers()) {
            template.convertAndSendToUser(member.getUserName(), QUEUE_LOBBY + lobby.getUuid(),
                    new SocketMessage(new LobbyMessageOut(lobby), null, null, MessageType.LOBBY_UPDATE, LocalDateTime.now()));
        }
    }

    public void sendLobbyUpdateForUserChangeIfLobbyExists(User user) {
        Optional<Lobby> lobby = lobbyService.getLobbyByUser(user);
        if (lobby.isPresent()) {
            for (User member : lobby.get().getMembers()) {
                if (member.equals(user)) {
                    member.setSummonerIconId(user.getSummonerIconId());
                }
            }

            for (User member : lobby.get().getMembers()) {
                template.convertAndSendToUser(member.getUserName(), QUEUE_LOBBY + lobby.get().getUuid(),
                        new SocketMessage(new LobbyMessageOut(lobby.get()), null, null, MessageType.LOBBY_UPDATE, LocalDateTime.now()));
            }
        }
    }

    public Optional<Lobby> getLobbyByUser(User user) {
        return lobbyService.getLobbyByUser(user);
    }
}

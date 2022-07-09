package de.jandev.ls4apiserver.websocket;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.exception.ApplicationExceptionCode;
import de.jandev.ls4apiserver.model.lobby.Lobby;
import de.jandev.ls4apiserver.model.user.User;
import de.jandev.ls4apiserver.model.websocket.ErrorMessage;
import de.jandev.ls4apiserver.model.websocket.SocketMessage;
import de.jandev.ls4apiserver.model.websocket.lobby.LobbyTypeIn;
import de.jandev.ls4apiserver.service.UserService;
import de.jandev.ls4apiserver.service.utility.LogMessage;
import de.jandev.ls4apiserver.websocket.handler.LobbyMessageHandler;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.HttpStatus;
import org.springframework.messaging.handler.annotation.Header;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.stereotype.Controller;

import java.security.Principal;
import java.time.LocalDateTime;
import java.util.Optional;

@Controller
public class LobbySocketController {

    private static final Logger LOGGER = LoggerFactory.getLogger(LobbySocketController.class);
    private static final String QUEUE_LOBBY = "/queue/lobby";
    private final SimpMessagingTemplate template;
    private final UserService userService;
    private final LobbyMessageHandler lobbyMessageHandler;
    private final ObjectMapper mapper;

    public LobbySocketController(SimpMessagingTemplate template, UserService userService, LobbyMessageHandler lobbyMessageHandler, ObjectMapper mapper) {
        this.template = template;
        this.userService = userService;
        this.lobbyMessageHandler = lobbyMessageHandler;
        this.mapper = mapper;
    }

    public void init(String lobbyUuid, User user) {
        try {
            lobbyMessageHandler.initLobby(lobbyUuid, user);
        } catch (ApplicationException e) {
            template.convertAndSendToUser(user.getUserName(), QUEUE_LOBBY, new SocketMessage(null, null, new ErrorMessage(e.getHttpStatus().value(), e.getCode(), e.getMessage()), null, LocalDateTime.now()));
        }
    }

    public void lostConnection(User user) {
        lobbyMessageHandler.handleLostConnection(user);
    }

    @MessageMapping("/lobby")
    public void incomingLobbyMessage(@Payload SocketMessage message, Principal userPrincipal, @Header("simpSessionId") String sessionId) {
        LOGGER.info(LogMessage.WEBSOCKET_MESSAGE_RECEIVED, message.getMessageType(), message.getId(), userPrincipal.getName(), message.getData(), QUEUE_LOBBY);

        User user;
        try {
            user = userService.getUserByUserName(userPrincipal.getName());
        } catch (ApplicationException e) {
            // We don't handle that, because we cannot do a callback for this error, because we don't have the lobby uuid yet
            return;
        }

        Optional<Lobby> lobby = lobbyMessageHandler.getLobbyByUser(user);

        // If it's not present, the request should not have been sent.
        if (lobby.isPresent()) {
            try {
                switch (message.getMessageType()) {
                    case LOBBY_INVITE:
                        lobbyMessageHandler.handleLobbyInvite(lobby.get(), user, userService.getUserBySummonerName((String) message.getData()));
                        break;
                    case LOBBY_CHANGE_TYPE:
                        var lobbyTypeIn = mapper.readValue(mapper.writeValueAsString(message.getData()), LobbyTypeIn.class);
                        lobbyMessageHandler.handleLobbyChangeLobbyType(lobby.get(), user, lobbyTypeIn);
                        break;
                    case LOBBY_KICK:
                        lobbyMessageHandler.handleLobbyKick(lobby.get(), user, userService.getUserBySummonerName((String) message.getData()));
                        break;
                    case LOBBY_CHAT:
                        lobbyMessageHandler.handleLobbyChat(lobby.get(), user, (String) message.getData());
                        break;
                    case LOBBY_LEAVE:
                        lobbyMessageHandler.handleLobbyLeave(lobby.get(), user);
                        break;
                    case LOBBY_MATCHMAKING_START:
                        lobbyMessageHandler.handleLobbyMatchmakingStart(lobby.get(), user);
                        break;
                    case LOBBY_MATCHMAKING_STOP:
                        lobbyMessageHandler.handleLobbyMatchmakingStop(lobby.get());
                        break;
                    case LOBBY_SWITCH_TEAM:
                        lobbyMessageHandler.handleLobbySwitchTeam(lobby.get(), user);
                        break;
                    case LOBBY_CHAMPSELECT_ACCEPT:
                        lobbyMessageHandler.handleLobbyUserMatchmakingReply(lobby.get(), user, true);
                        break;
                    case LOBBY_CHAMPSELECT_DENY:
                        lobbyMessageHandler.handleLobbyUserMatchmakingReply(lobby.get(), user, false);
                        break;
                    default:
                        break;
                }

                template.convertAndSendToUser(user.getUserName(), QUEUE_LOBBY + lobby.get().getUuid(), new SocketMessage(null, message.getId(), null, message.getMessageType(), LocalDateTime.now()));
            } catch (ApplicationException e) {
                template.convertAndSendToUser(userPrincipal.getName(), QUEUE_LOBBY + lobby.get().getUuid(),
                        new SocketMessage(null, message.getId(), new ErrorMessage(e.getHttpStatus().value(), e.getCode(), e.getMessage()), message.getMessageType(), LocalDateTime.now()));
            } catch (JsonProcessingException e) {
                template.convertAndSendToUser(userPrincipal.getName(), QUEUE_LOBBY + lobby.get().getUuid(),
                        new SocketMessage(null, message.getId(), new ErrorMessage(HttpStatus.BAD_REQUEST.value(), ApplicationExceptionCode.REQUEST_NOT_READABLE.getCode(), LogMessage.REQUEST_NOT_READABLE), message.getMessageType(), LocalDateTime.now()));
            } catch (Exception e) {
                LOGGER.error(LogMessage.UNHANDLED_EXCEPTION, e);
                template.convertAndSendToUser(userPrincipal.getName(), QUEUE_LOBBY + lobby.get().getUuid(),
                        new SocketMessage(null, message.getId(), new ErrorMessage(HttpStatus.INTERNAL_SERVER_ERROR.value(), ApplicationExceptionCode.UNHANDLED_EXCEPTION.getCode(), LogMessage.UNHANDLED_EXCEPTION), message.getMessageType(), LocalDateTime.now()));
            }
        }
    }
}

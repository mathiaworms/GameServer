package de.jandev.ls4apiserver.websocket;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.exception.ApplicationExceptionCode;
import de.jandev.ls4apiserver.model.champselect.GameLobby;
import de.jandev.ls4apiserver.model.champselect.LobbyUser;
import de.jandev.ls4apiserver.model.user.User;
import de.jandev.ls4apiserver.model.websocket.ErrorMessage;
import de.jandev.ls4apiserver.model.websocket.SocketMessage;
import de.jandev.ls4apiserver.service.utility.LogMessage;
import de.jandev.ls4apiserver.websocket.handler.ChampselectMessageHandler;
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
public class ChampselectSocketController {

    private static final Logger LOGGER = LoggerFactory.getLogger(ChampselectSocketController.class);
    private static final String QUEUE_CHAMPSELECT = "/queue/champselect";
    private final SimpMessagingTemplate template;
    private final ChampselectMessageHandler champselectMessageHandler;

    public ChampselectSocketController(SimpMessagingTemplate template, ChampselectMessageHandler champselectMessageHandler) {
        this.template = template;
        this.champselectMessageHandler = champselectMessageHandler;
    }

    public void lostConnection(User user) {
        champselectMessageHandler.handleLostConnection(user);
    }

    @MessageMapping("/champselect")
    public void incomingChampselectMessage(@Payload SocketMessage message, Principal userPrincipal, @Header("simpSessionId") String sessionId) {
        LOGGER.info(LogMessage.WEBSOCKET_MESSAGE_RECEIVED, message.getMessageType(), message.getId(), userPrincipal.getName(), message.getData(), QUEUE_CHAMPSELECT);

        Optional<GameLobby> gameLobby = champselectMessageHandler.getGameLobbyByUserName(userPrincipal.getName());

        // If it's not present, the request should not have been sent.
        if (gameLobby.isPresent()) {
            try {
                Optional<LobbyUser> lobbyUser = champselectMessageHandler.getLobbyUserFromGameLobbyByUsername(gameLobby.get(), userPrincipal.getName());

                if (lobbyUser.isPresent()) {
                    switch (message.getMessageType()) {
                        case CHAMPSELECT_CHAT:
                            champselectMessageHandler.handleChat(gameLobby.get(), lobbyUser.get(), (String) message.getData());
                            break;
                        case CHAMPSELECT_SELECT_CHAMPION:
                            champselectMessageHandler.handleSelectChampion(gameLobby.get(), lobbyUser.get(), (String) message.getData());
                            break;
                        case CHAMPSELECT_BAN_SELECT_CHAMPION:
                            champselectMessageHandler.handleBanSelectChampion(gameLobby.get(), lobbyUser.get(), (String) message.getData());
                            break;
                        case CHAMPSELECT_BAN_LOCK_CHAMPION:
                            champselectMessageHandler.handleBanLockChampion(gameLobby.get(), lobbyUser.get(), (String) message.getData());
                            break;
                        case CHAMPSELECT_LOCK_CHAMPION:
                            champselectMessageHandler.handleLockChampion(gameLobby.get(), lobbyUser.get(), (String) message.getData());
                            break;
                        case CHAMPSELECT_SELECT_SPELL_1:
                            champselectMessageHandler.handleSelectSpell(gameLobby.get(), lobbyUser.get(), (String) message.getData(), true);
                            break;
                        case CHAMPSELECT_SELECT_SPELL_2:
                            champselectMessageHandler.handleSelectSpell(gameLobby.get(), lobbyUser.get(), (String) message.getData(), false);
                            break;
                        case CHAMPSELECT_SELECT_SKIN:
                            champselectMessageHandler.handleSelectSkin(gameLobby.get(), lobbyUser.get(), (String) message.getData());
                            break;
                        case CHAMPSELECT_TRADE_REQUEST:
                            champselectMessageHandler.handleTradeRequest(gameLobby.get(), lobbyUser.get(), (String) message.getData());
                            break;
                        case CHAMPSELECT_TRADE_ACCEPT:
                            champselectMessageHandler.handleTradeRequestReply(gameLobby.get(), lobbyUser.get(), (String) message.getData(), true);
                            break;
                        case CHAMPSELECT_TRADE_DENY:
                            champselectMessageHandler.handleTradeRequestReply(gameLobby.get(), lobbyUser.get(), (String) message.getData(), false);
                            break;
                        default:
                            break;
                    }
                } else {
                    template.convertAndSendToUser(userPrincipal.getName(), QUEUE_CHAMPSELECT + gameLobby.get().getUuid(),
                            new SocketMessage(null, message.getId(), new ErrorMessage(HttpStatus.NOT_FOUND.value(), ApplicationExceptionCode.CHAMPSELECT_USER_NOT_FOUND.getCode(), LogMessage.CHAMPSELECT_USER_NOT_FOUND), message.getMessageType(), LocalDateTime.now()));
                }
                template.convertAndSendToUser(userPrincipal.getName(), QUEUE_CHAMPSELECT + gameLobby.get().getUuid(),
                        new SocketMessage(null, message.getId(), null, message.getMessageType(), LocalDateTime.now()));
            } catch (ApplicationException e) {
                template.convertAndSendToUser(userPrincipal.getName(), QUEUE_CHAMPSELECT + gameLobby.get().getUuid(),
                        new SocketMessage(null, message.getId(), new ErrorMessage(e.getHttpStatus().value(), e.getCode(), e.getMessage()), message.getMessageType(), LocalDateTime.now()));
            } catch (Exception e) {
                LOGGER.error(LogMessage.UNHANDLED_EXCEPTION, e);
                template.convertAndSendToUser(userPrincipal.getName(), QUEUE_CHAMPSELECT + gameLobby.get().getUuid(),
                        new SocketMessage(null, message.getId(), new ErrorMessage(HttpStatus.INTERNAL_SERVER_ERROR.value(), ApplicationExceptionCode.UNHANDLED_EXCEPTION.getCode(), LogMessage.UNHANDLED_EXCEPTION), message.getMessageType(), LocalDateTime.now()));
            }
        }
    }
}

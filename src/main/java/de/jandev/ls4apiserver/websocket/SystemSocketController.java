package de.jandev.ls4apiserver.websocket;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.exception.ApplicationExceptionCode;
import de.jandev.ls4apiserver.model.chat.ChatMessage;
import de.jandev.ls4apiserver.model.user.Relationship;
import de.jandev.ls4apiserver.model.user.RelationshipStatus;
import de.jandev.ls4apiserver.model.user.SummonerStatus;
import de.jandev.ls4apiserver.model.user.User;
import de.jandev.ls4apiserver.model.user.dto.UserPublicOut;
import de.jandev.ls4apiserver.model.websocket.ErrorMessage;
import de.jandev.ls4apiserver.model.websocket.MessageType;
import de.jandev.ls4apiserver.model.websocket.SocketMessage;
import de.jandev.ls4apiserver.model.websocket.chat.ChatMessageIn;
import de.jandev.ls4apiserver.model.websocket.chat.ChatMessageOut;
import de.jandev.ls4apiserver.model.websocket.lobby.LobbyTypeIn;
import de.jandev.ls4apiserver.model.websocket.user.SummonerNameUpdateOut;
import de.jandev.ls4apiserver.service.ChatService;
import de.jandev.ls4apiserver.service.UserService;
import de.jandev.ls4apiserver.service.utility.LogMessage;
import de.jandev.ls4apiserver.websocket.handler.FriendMessageHandler;
import de.jandev.ls4apiserver.websocket.handler.LobbyMessageHandler;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.HttpStatus;
import org.springframework.messaging.handler.annotation.Header;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.stereotype.Controller;
import org.springframework.transaction.annotation.Transactional;

import java.security.Principal;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;

@Controller
@Transactional
public class SystemSocketController {

    private static final Logger LOGGER = LoggerFactory.getLogger(SystemSocketController.class);
    private static final String QUEUE_SYSTEM = "/queue/system";
    private final SimpMessagingTemplate template;
    private final UserService userService;
    private final ChatService chatService;
    private final FriendMessageHandler friendMessageHandler;
    private final LobbyMessageHandler lobbyMessageHandler;
    private final ObjectMapper mapper;

    public SystemSocketController(SimpMessagingTemplate template, UserService userService, ChatService chatService, FriendMessageHandler friendMessageHandler, LobbyMessageHandler lobbyMessageHandler, ObjectMapper mapper) {
        this.template = template;
        this.userService = userService;
        this.chatService = chatService;
        this.friendMessageHandler = friendMessageHandler;
        this.lobbyMessageHandler = lobbyMessageHandler;
        this.mapper = mapper;
    }

    public void init(User user, boolean loggedInTwice) {
        if (loggedInTwice) {
            template.convertAndSendToUser(user.getUserName(), QUEUE_SYSTEM,
                    new SocketMessage(null, null, new ErrorMessage(HttpStatus.CONFLICT.value(), ApplicationExceptionCode.USER_LOGGED_IN_ANOTHER_LOCATION.getCode(), LogMessage.USER_LOGGED_IN_ANOTHER_LOCATION), MessageType.USER_LOGGED_IN_ANOTHER_LOCATION, LocalDateTime.now()));
        }

        List<SocketMessage> socketMessageList = new ArrayList<>();

        friendMessageHandler.initFriendLists(user, socketMessageList);

        user.setSummonerStatus(SummonerStatus.ONLINE);
        sendFriendUpdate(user);

        for (SocketMessage message : socketMessageList) {
            template.convertAndSendToUser(user.getUserName(), QUEUE_SYSTEM, message);
        }
    }

    @MessageMapping("/system")
    public void incomingSystemMessage(@Payload SocketMessage message, Principal userPrincipal, @Header("simpSessionId") String sessionId) {
        LOGGER.info(LogMessage.WEBSOCKET_MESSAGE_RECEIVED, message.getMessageType(), message.getId(), userPrincipal.getName(), message.getData(), QUEUE_SYSTEM);

        try {
            var user = userService.getUserByUserName(userPrincipal.getName());

            var callBackHandled = false;

            switch (message.getMessageType()) {
                case FRIEND_OUT:
                    friendMessageHandler.handleFriendInvite(user, userService.getUserBySummonerName((String) message.getData()));
                    break;
                case FRIEND_IN_ACCEPT:
                    friendMessageHandler.handleFriendInviteAccept(user, userService.getUserBySummonerName((String) message.getData()));
                    break;
                case FRIEND_IN_DENY:
                    friendMessageHandler.handleFriendInviteDeny(user, userService.getUserBySummonerName((String) message.getData()));
                    break;
                case FRIEND_REMOVE:
                    friendMessageHandler.handleFriendRemove(user, userService.getUserBySummonerName((String) message.getData()));
                    break;
                case FRIEND_BLOCK:
                    friendMessageHandler.handleFriendBlock(user, userService.getUserBySummonerName((String) message.getData()));
                    break;
                case MESSAGE_PRIVATE:
                    ChatMessageIn cm = mapper.readValue(mapper.writeValueAsString(message.getData()), ChatMessageIn.class);
                    handleMessagePrivate(user, userService.getUserBySummonerName(cm.getTo()), cm.getData());
                    break;
                case MESSAGE_PRIVATE_GET:
                    callBackHandled = true;
                    handleMessagePrivateGet(message.getId(), user, userService.getUserBySummonerName((String) message.getData()));
                    break;
                case USER_UPDATE_ICON:
                    updateSummonerIcon(user, message.getData());
                    break;
                case USER_UPDATE_STATUS:
                    updateStatus(user, (String) message.getData());
                    break;
                case USER_UPDATE_MOTTO:
                    updateMotto(user, (String) message.getData());
                    break;
                case USER_BAN:
                    banUser(user, (String) message.getData());
                    break;
                // We handle this in this controller, because we are sending the response to the system queue anyway. Handling it here makes error handling simpler.
                case LOBBY_CREATE:
                    callBackHandled = true;
                    var lobbyTypeIn = mapper.readValue(mapper.writeValueAsString(message.getData()), LobbyTypeIn.class);
                    lobbyMessageHandler.handleLobbyCreate(message.getId(), user, lobbyTypeIn);
                    break;
                case LOBBY_ACCEPT:
                    callBackHandled = true;
                    lobbyMessageHandler.handleLobbyInviteAccept(message.getId(), (String) message.getData(), user);
                    break;
                case LOBBY_DENY:
                    lobbyMessageHandler.handleLobbyInviteDeny((String) message.getData(), user);
                    break;
                default:
                    break;
            }

            if (!callBackHandled) {
                template.convertAndSendToUser(user.getUserName(), QUEUE_SYSTEM, new SocketMessage(null, message.getId(), null, message.getMessageType(), LocalDateTime.now()));
            }
        } catch (ApplicationException e) {
            template.convertAndSendToUser(userPrincipal.getName(), QUEUE_SYSTEM,
                    new SocketMessage(null, message.getId(), new ErrorMessage(e.getHttpStatus().value(), e.getCode(), e.getMessage()), message.getMessageType(), LocalDateTime.now()));
        } catch (JsonProcessingException e) {
            template.convertAndSendToUser(userPrincipal.getName(), QUEUE_SYSTEM,
                    new SocketMessage(null, message.getId(), new ErrorMessage(HttpStatus.BAD_REQUEST.value(), ApplicationExceptionCode.REQUEST_NOT_READABLE.getCode(), LogMessage.REQUEST_NOT_READABLE), message.getMessageType(), LocalDateTime.now()));
        } catch (Exception e) {
            LOGGER.error(LogMessage.UNHANDLED_EXCEPTION, e);
            template.convertAndSendToUser(userPrincipal.getName(), QUEUE_SYSTEM,
                    new SocketMessage(null, message.getId(), new ErrorMessage(HttpStatus.INTERNAL_SERVER_ERROR.value(), ApplicationExceptionCode.UNHANDLED_EXCEPTION.getCode(), LogMessage.UNHANDLED_EXCEPTION), message.getMessageType(), LocalDateTime.now()));
        }
    }

    private void handleMessagePrivate(User user, User target, String message) throws ApplicationException {
        chatService.createChatMessage(user, target, message);
        template.convertAndSendToUser(target.getUserName(), QUEUE_SYSTEM,
                new SocketMessage(new ChatMessageOut(user.getSummonerName(), target.getSummonerName(), message, LocalDateTime.now()), null, null, MessageType.MESSAGE_PRIVATE, LocalDateTime.now()));
    }

    public void handleMessagePrivateGet(String id, User user, User target) {
        List<ChatMessage> messages = chatService.getChatMessagesWithFriend(user, target);
        List<ChatMessageOut> messagesOut = messages.stream()
                .map(c -> new ChatMessageOut(c.getFrom().getSummonerName(), c.getTo().getSummonerName(), c.getData(), c.getMessageTimestamp())).collect(Collectors.toList());
        template.convertAndSendToUser(user.getUserName(), QUEUE_SYSTEM, new SocketMessage(messagesOut, id, null, MessageType.MESSAGE_PRIVATE_GET, LocalDateTime.now()));
    }

    public void sendFriendUpdate(User sender) {
        friendMessageHandler.sendFriendUpdate(sender);
    }

    public void updateMotto(User user, String userMotto) throws ApplicationException {
        userService.updateMotto(user, userMotto);
        sendFriendUpdate(user);
    }

    public void updateSummonerIcon(User user, Object userIcon) throws ApplicationException {
        userService.updateSummonerIcon(user, userIcon);
        sendFriendUpdate(user);

        lobbyMessageHandler.sendLobbyUpdateForUserChangeIfLobbyExists(user);
    }

    public void updateStatus(User user, String status) throws ApplicationException {
        userService.updateStatus(user, status);
        sendFriendUpdate(user);
    }

    public void banUser(User user, String summonerName) throws ApplicationException {
        var bannedUser = userService.banUserBySummonerName(user, summonerName);

        bannedUser.setSummonerStatus(SummonerStatus.OFFLINE);

        List<Relationship> relationships = new ArrayList<>(bannedUser.getFriends());

        for (Relationship relationship : relationships) {
            userService.removeFriend(bannedUser, relationship.getFriend());

            template.convertAndSendToUser(relationship.getFriend().getUserName(), QUEUE_SYSTEM,
                    new SocketMessage(new UserPublicOut(bannedUser), null, null, MessageType.FRIEND_REMOVE, LocalDateTime.now()));
        }

        template.convertAndSendToUser(bannedUser.getUserName(), QUEUE_SYSTEM,
                new SocketMessage(null, null, null, MessageType.USER_BAN, LocalDateTime.now()));
    }

    public void forceDisconnectAndUpdateSummonerName(String oldSummonerName, User user) {
        template.convertAndSendToUser(user.getUserName(), QUEUE_SYSTEM,
                new SocketMessage(null, null, null, MessageType.KILL, LocalDateTime.now()));

        for (User friend : user.getFriends().stream().filter(c -> c.getRelationshipStatus() == RelationshipStatus.FRIEND).map(Relationship::getFriend).collect(Collectors.toList())) {
            template.convertAndSendToUser(friend.getUserName(), QUEUE_SYSTEM, new SocketMessage(new SummonerNameUpdateOut(oldSummonerName, user.getSummonerName()), null, null, MessageType.USER_UPDATE_SUMMONER_NAME, LocalDateTime.now()));
        }
    }
}

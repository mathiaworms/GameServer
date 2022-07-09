package de.jandev.ls4apiserver.websocket.handler;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.exception.ApplicationExceptionCode;
import de.jandev.ls4apiserver.model.user.Relationship;
import de.jandev.ls4apiserver.model.user.RelationshipStatus;
import de.jandev.ls4apiserver.model.user.User;
import de.jandev.ls4apiserver.model.user.dto.UserPublicOut;
import de.jandev.ls4apiserver.model.websocket.MessageType;
import de.jandev.ls4apiserver.model.websocket.SocketMessage;
import de.jandev.ls4apiserver.service.UserService;
import de.jandev.ls4apiserver.service.utility.LogMessage;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.slf4j.helpers.MessageFormatter;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.HttpStatus;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;

@Component
@Transactional
public class FriendMessageHandler {
    private static final Logger LOGGER = LoggerFactory.getLogger(FriendMessageHandler.class);
    private static final String QUEUE_SYSTEM = "/queue/system";
    private final SimpMessagingTemplate template;
    private final UserService userService;
    @Value("${user.max.friends}")
    private int maxFriends;
    @Value("${user.max.friend.requests.in}")
    private int maxFriendRequestsIn;
    @Value("${user.max.friend.requests.out}")
    private int maxFriendRequestsOut;

    public FriendMessageHandler(SimpMessagingTemplate template, UserService userService) {
        this.template = template;
        this.userService = userService;
    }

    public void initFriendLists(User user, List<SocketMessage> socketMessageList) {
        List<UserPublicOut> userFriendInList = new ArrayList<>();
        List<UserPublicOut> userFriendOutList = new ArrayList<>();
        List<UserPublicOut> userFriendList = new ArrayList<>();

        getFriends(user, userFriendInList, userFriendOutList, userFriendList);

        socketMessageList.add(new SocketMessage(userFriendInList, null, null, MessageType.FRIEND_IN, LocalDateTime.now()));
        socketMessageList.add(new SocketMessage(userFriendOutList, null, null, MessageType.FRIEND_OUT, LocalDateTime.now()));
        socketMessageList.add(new SocketMessage(userFriendList, null, null, MessageType.FRIEND_LIST, LocalDateTime.now()));
    }

    public void getFriends(User user, List<UserPublicOut> userFriendInList, List<UserPublicOut> userFriendOutList, List<UserPublicOut> userFriendList) {
        for (Relationship relationship : user.getFriends()) {
            if (relationship.getRelationshipStatus() == RelationshipStatus.FRIEND) {
                userFriendList.add(new UserPublicOut(relationship.getFriend()));
            } else if (relationship.getRelationshipStatus() == RelationshipStatus.PENDING) {
                userFriendOutList.add(new UserPublicOut(relationship.getFriend()));
            }
        }

        userFriendInList.addAll(user.getFriendedBy().stream().filter(c -> c.getRelationshipStatus() == RelationshipStatus.PENDING).map(c -> new UserPublicOut(c.getOwner())).collect(Collectors.toList()));
    }

    public void handleFriendInvite(User user, User target) throws ApplicationException {
        // If target already sent a friend request, sent FRIEND_IN_ACCEPT out
        if (user.getFriendedBy().stream().anyMatch(u -> u.getOwner().equals(target) && u.getRelationshipStatus() == RelationshipStatus.PENDING)) {
            addFriend(user, target,
                    new SocketMessage(new UserPublicOut(user), null, null, MessageType.FRIEND_IN_ACCEPT, LocalDateTime.now()));
        } else {
            addFriend(user, target,
                    new SocketMessage(new UserPublicOut(user), null, null, MessageType.FRIEND_IN, LocalDateTime.now()));
        }
    }

    public void handleFriendInviteAccept(User user, User target) throws ApplicationException {
        addFriend(user, target,
                new SocketMessage(new UserPublicOut(user), null, null, MessageType.FRIEND_IN_ACCEPT, LocalDateTime.now()));
    }

    public void addFriend(User user, User target, SocketMessage socketMessage) throws ApplicationException {
        checkFriendLimits(user, target);

        userService.addFriend(user, target);
        template.convertAndSendToUser(target.getUserName(), QUEUE_SYSTEM, socketMessage);
    }

    public void handleFriendRemove(User user, User target) throws ApplicationException {
        userService.removeFriend(user, target);
        template.convertAndSendToUser(target.getUserName(), QUEUE_SYSTEM,
                new SocketMessage(new UserPublicOut(user), null, null, MessageType.FRIEND_REMOVE, LocalDateTime.now()));
    }

    public void handleFriendBlock(User user, User target) throws ApplicationException {
        userService.blockFriend(user, target);
        template.convertAndSendToUser(target.getUserName(), QUEUE_SYSTEM,
                new SocketMessage(new UserPublicOut(user), null, null, MessageType.FRIEND_REMOVE, LocalDateTime.now()));
    }

    public void handleFriendInviteDeny(User user, User target) throws ApplicationException {
        userService.denyFriendRequest(user, target);
        template.convertAndSendToUser(target.getUserName(), QUEUE_SYSTEM,
                new SocketMessage(new UserPublicOut(user), null, null, MessageType.FRIEND_IN_DENY, LocalDateTime.now()));
    }

    public void sendFriendUpdate(User sender) {
        for (User friend : sender.getFriends().stream().filter(c -> c.getRelationshipStatus() == RelationshipStatus.FRIEND).map(Relationship::getFriend).collect(Collectors.toList())) {
            template.convertAndSendToUser(friend.getUserName(), QUEUE_SYSTEM, new SocketMessage(new UserPublicOut(sender), null, null, MessageType.FRIEND_UPDATE, LocalDateTime.now()));
        }
    }

    public void checkFriendLimits(User user, User target) throws ApplicationException {
        if (user.getFriends().size() >= maxFriends) {
            // Check if friend limit is exceeded
            LOGGER.info(LogMessage.FRIEND_LIMIT_EXCEEDED, target.getUserName());
            throw new ApplicationException(HttpStatus.CONFLICT, ApplicationExceptionCode.FRIEND_LIMIT_EXCEEDED, MessageFormatter.format(LogMessage.FRIEND_LIMIT_EXCEEDED, target.getSummonerName()).getMessage());
        } else if (target.getFriends().size() >= maxFriends) {
            LOGGER.info(LogMessage.FRIEND_LIMIT_EXCEEDED_TARGET, target.getUserName());
            throw new ApplicationException(HttpStatus.CONFLICT, ApplicationExceptionCode.FRIEND_LIMIT_EXCEEDED_TARGET, MessageFormatter.format(LogMessage.FRIEND_LIMIT_EXCEEDED_TARGET, target.getSummonerName()).getMessage());
        } else if (target.getFriendedBy().stream().filter(u -> u.getRelationshipStatus() == RelationshipStatus.PENDING).count() >= maxFriendRequestsIn) {
            // Check if targets incoming pending request list is full
            LOGGER.info(LogMessage.FRIEND_REQUEST_LIMIT_EXCEEDED_TARGET, target.getUserName());
            throw new ApplicationException(HttpStatus.CONFLICT, ApplicationExceptionCode.FRIEND_REQUEST_LIMIT_EXCEEDED_TARGET, MessageFormatter.format(LogMessage.FRIEND_REQUEST_LIMIT_EXCEEDED_TARGET, target.getSummonerName()).getMessage());
        } else if (user.getFriends().stream().filter(u -> u.getRelationshipStatus() == RelationshipStatus.PENDING).count() >= maxFriendRequestsOut) {
            // Check if user outgoing pending lists are too full
            LOGGER.info(LogMessage.FRIEND_REQUEST_LIMIT_EXCEEDED, target.getUserName());
            throw new ApplicationException(HttpStatus.CONFLICT, ApplicationExceptionCode.FRIEND_REQUEST_LIMIT_EXCEEDED, MessageFormatter.format(LogMessage.FRIEND_REQUEST_LIMIT_EXCEEDED, target.getSummonerName()).getMessage());
        }
    }
}

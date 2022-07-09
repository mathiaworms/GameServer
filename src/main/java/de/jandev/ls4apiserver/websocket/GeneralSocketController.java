package de.jandev.ls4apiserver.websocket;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.model.user.SummonerStatus;
import de.jandev.ls4apiserver.model.user.User;
import de.jandev.ls4apiserver.service.UserService;
import de.jandev.ls4apiserver.service.utility.LogMessage;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.event.EventListener;
import org.springframework.core.annotation.Order;
import org.springframework.messaging.simp.SimpMessageHeaderAccessor;
import org.springframework.messaging.simp.user.SimpSession;
import org.springframework.messaging.simp.user.SimpSubscription;
import org.springframework.messaging.simp.user.SimpUser;
import org.springframework.messaging.simp.user.SimpUserRegistry;
import org.springframework.stereotype.Controller;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.socket.messaging.SessionDisconnectEvent;
import org.springframework.web.socket.messaging.SessionSubscribeEvent;
import org.springframework.web.socket.messaging.SessionUnsubscribeEvent;

import javax.annotation.PostConstruct;
import java.security.Principal;
import java.util.Objects;

@Controller
@Transactional
public class GeneralSocketController {

    private static final Logger LOGGER = LoggerFactory.getLogger(GeneralSocketController.class);
    private final UserService userService;
    private final SystemSocketController systemSocketController;
    private final LobbySocketController lobbySocketController;
    private final ChampselectSocketController champselectSocketController;
    private final SimpUserRegistry simpUserRegistry;

    public GeneralSocketController(UserService userService, SystemSocketController systemSocketController, LobbySocketController lobbySocketController, ChampselectSocketController champselectSocketController, SimpUserRegistry simpUserRegistry) {
        this.userService = userService;
        this.systemSocketController = systemSocketController;
        this.lobbySocketController = lobbySocketController;
        this.champselectSocketController = champselectSocketController;
        this.simpUserRegistry = simpUserRegistry;
    }

    @PostConstruct
    public void resetUserSummonerStatus() {
        userService.changeAllUsersToOffline();
    }

    @EventListener
    public void handleSessionSubscribe(SessionSubscribeEvent event) throws ApplicationException {
        var principal = event.getUser();
        var header = SimpMessageHeaderAccessor.wrap(event.getMessage());
        String destination = header.getDestination();
        if (principal != null && destination != null) {
            var user = userService.getUserByUserName(principal.getName());

            LOGGER.info(LogMessage.WEBSOCKET_CONNECTED_QUEUE, user.getUserName(), destination);

            // Header should always be set, also the user should always be set, because spring security authenticates him
            if (Objects.equals(destination, "/user/queue/system")) {
                var simpUser = simpUserRegistry.getUser(principal.getName());
                systemSocketController.init(user, simpUser != null && simpUser.getSessions().size() > 1);
            }

            // Redundant, because we send the lobby object on join / create, but we do it just to be safe.
            if (destination.contains("/user/queue/lobby")) {
                user.setSummonerStatus(SummonerStatus.IN_LOBBY);

                String[] lobbyUuid = destination.split("lobby");
                if (lobbyUuid.length >= 2) {
                    lobbySocketController.init(lobbyUuid[1], user);
                }

                systemSocketController.sendFriendUpdate(user);
            }

            if (destination.contains("/user/queue/champselect")) {
                user.setSummonerStatus(SummonerStatus.IN_CHAMP_SELECT);
                systemSocketController.sendFriendUpdate(user);
            }
        }
    }

    @EventListener
    @Order(1)
    public void handleSessionUnsubscribe(SessionUnsubscribeEvent event) throws ApplicationException {
        // Fixed this with Order, but we can also keep track of the subscriptionid+sessionid and destination in a ConcurrentHashMap.
        var principal = event.getUser();
        var header = SimpMessageHeaderAccessor.wrap(event.getMessage());
        String subscriptionId = header.getSubscriptionId();

        if (principal != null && subscriptionId != null) {
            var simpUser = simpUserRegistry.getUser(principal.getName());

            if (simpUser != null) {
                checkUnsubscribeSession(principal, subscriptionId, simpUser);
            }
        }
    }

    public void checkUnsubscribeSession(Principal principal, String subscriptionId, SimpUser simpUser) throws ApplicationException {
        var user = userService.getUserByUserName(principal.getName());

        // Should only have one session, but can have multiple temporarily
        for (SimpSession session : simpUser.getSessions()) {
            for (SimpSubscription subscription : session.getSubscriptions()) {
                if (subscription.getDestination().contains("/user/queue/lobby") && subscription.getId().equals(subscriptionId)) {
                    user.setSummonerStatus(SummonerStatus.ONLINE);
                    systemSocketController.sendFriendUpdate(user);

                    lobbySocketController.lostConnection(user);

                    LOGGER.info(LogMessage.WEBSOCKET_DISCONNECTED_QUEUE, user.getUserName(), subscription.getDestination());
                } else if (subscription.getDestination().contains("/user/queue/champselect") && subscription.getId().equals(subscriptionId)) {
                    user.setSummonerStatus(SummonerStatus.IN_LOBBY);
                    systemSocketController.sendFriendUpdate(user);

                    // Will not be triggered as the unsubscribe is sent after game start, but just in case.
                    champselectSocketController.lostConnection(user);

                    LOGGER.info(LogMessage.WEBSOCKET_DISCONNECTED_QUEUE, user.getUserName(), subscription.getDestination());
                }
            }
        }
    }

    @EventListener
    public void handleSessionDisconnect(SessionDisconnectEvent event) throws ApplicationException {
        var principal = event.getUser();
        if (principal != null) {
            var user = userService.getUserByUserName(principal.getName());

            user.setSummonerStatus(SummonerStatus.OFFLINE);

            systemSocketController.sendFriendUpdate(user);

            champselectSocketController.lostConnection(user);

            lobbySocketController.lostConnection(user);

            LOGGER.info(LogMessage.WEBSOCKET_DISCONNECTED, user.getUserName());
        }
    }

    public void forceDisconnectAndUpdateSummonerName(String oldSummonerName, User user) {
        user.setSummonerStatus(SummonerStatus.OFFLINE);

        champselectSocketController.lostConnection(user);

        lobbySocketController.lostConnection(user);

        systemSocketController.forceDisconnectAndUpdateSummonerName(oldSummonerName, user);

        systemSocketController.sendFriendUpdate(user);

        LOGGER.info(LogMessage.WEBSOCKET_DISCONNECTED_FORCE, user.getUserName());
    }
}

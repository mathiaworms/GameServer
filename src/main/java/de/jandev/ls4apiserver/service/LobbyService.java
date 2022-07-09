package de.jandev.ls4apiserver.service;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.exception.ApplicationExceptionCode;
import de.jandev.ls4apiserver.model.champselect.LobbyTeam;
import de.jandev.ls4apiserver.model.event.LobbyRemoveEvent;
import de.jandev.ls4apiserver.model.lobby.InviteStatus;
import de.jandev.ls4apiserver.model.lobby.Lobby;
import de.jandev.ls4apiserver.model.lobby.LobbyType;
import de.jandev.ls4apiserver.model.user.User;
import de.jandev.ls4apiserver.model.websocket.lobby.LobbyTypeIn;
import de.jandev.ls4apiserver.service.utility.LogMessage;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.slf4j.helpers.MessageFormatter;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.ApplicationEventPublisher;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;

import java.util.*;
import java.util.concurrent.ConcurrentHashMap;

@Service
public class LobbyService {

    private static final Logger LOGGER = LoggerFactory.getLogger(LobbyService.class);
    private final Map<String, Lobby> lobbies = new ConcurrentHashMap<>();
    private final ApplicationEventPublisher applicationEventPublisher;
    @Value("${user.max.lobby.invites}")
    private int userMaxLobbyInvites;

    public LobbyService(ApplicationEventPublisher applicationEventPublisher) {
        this.applicationEventPublisher = applicationEventPublisher;
    }

    public Lobby createLobby(User user, LobbyTypeIn lobbyTypeIn) throws ApplicationException {
        Optional<Lobby> present = getLobbyByUser(user);
        if (present.isPresent()) {
            LOGGER.info(LogMessage.LOBBY_ALREADY_MEMBER, user.getUserName(), present.get().getUuid());
            throw new ApplicationException(HttpStatus.CONFLICT, ApplicationExceptionCode.LOBBY_ALREADY_MEMBER,
                    MessageFormatter.format(LogMessage.LOBBY_ALREADY_MEMBER, user.getUserName(), present.get().getUuid()).getMessage());
        }

        Optional<LobbyType> lobbyType = Arrays.stream(LobbyType.values()).filter(c -> c.name().equalsIgnoreCase(lobbyTypeIn.getLobbyType())).findFirst();

        if (lobbyType.isEmpty()) {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.LOBBY_TYPE_INVALID, MessageFormatter.format(LogMessage.LOBBY_TYPE_INVALID, lobbyTypeIn.getLobbyType()).getMessage());
        }

        var lobby = new Lobby();
        lobby.setOwner(user);
        lobby.setLobbyType(lobbyType.get());
        lobby.getMembers().add(user);

        if (lobbyTypeIn.isCustom()) {
            lobby.setCustom(lobbyTypeIn.isCustom());
        }

        setTeam(lobby, user, LobbyTeam.TEAM1);

        lobbies.put(lobby.getUuid(), lobby);
        LOGGER.info(LogMessage.LOBBY_CREATED, user.getUserName(), lobby.getUuid());

        return lobby;
    }

    public Lobby changeLobbyType(Lobby lobby, User user, LobbyTypeIn lobbyTypeIn) throws ApplicationException {
        isOwnerOrThrowException(user, lobby);

        Optional<LobbyType> lobbyType = Arrays.stream(LobbyType.values()).filter(c -> c.name().equalsIgnoreCase(lobbyTypeIn.getLobbyType())).findFirst();

        // Only allow type switch if lobby fits in new size
        // Custom games are always * 2 the size of the teamSize
        if (lobbyType.isPresent()
                && (lobby.getMembers().size() <= lobbyType.get().getTeamSize()
                || lobbyTypeIn.isCustom() && lobby.getMembers().size() <= (lobbyType.get().getTeamSize() * 2))) {

            lobby.setCustom(lobbyTypeIn.isCustom());

            lobby.setLobbyType(lobbyType.get());

            teamCheckTypeSwitch(lobby);

            LOGGER.info(LogMessage.LOBBY_TYPE_UPDATED, user.getUserName(), lobby.getUuid(), lobbyType.get());
        } else {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.LOBBY_TYPE_INVALID, MessageFormatter.format(LogMessage.LOBBY_TYPE_INVALID, lobbyTypeIn.getLobbyType()).getMessage());
        }

        return lobby;
    }

    public void isOwnerOrThrowException(User user, Lobby lobby) throws ApplicationException {
        if (!lobby.getOwner().equals(user)) {
            LOGGER.info(LogMessage.LOBBY_NO_OWNER, user.getUserName(), lobby.getUuid());
            throw new ApplicationException(HttpStatus.FORBIDDEN, ApplicationExceptionCode.LOBBY_NO_OWNER, MessageFormatter.format(LogMessage.LOBBY_NO_OWNER, user.getSummonerName(), lobby.getUuid()).getMessage());
        }
    }

    public Lobby removeMemberInternal(Lobby lobby, User user) {
        return removeMemberFromLobby(lobby, user);
    }

    public Lobby removeMember(Lobby lobby, User user) throws ApplicationException {
        if (lobby.getMembers().contains(user)) {
            return removeMemberFromLobby(lobby, user);
        } else {
            LOGGER.info(LogMessage.LOBBY_NO_MEMBER, user.getUserName(), lobby.getUuid());
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.LOBBY_NO_MEMBER, MessageFormatter.format(LogMessage.LOBBY_NO_MEMBER, user.getSummonerName(), lobby.getUuid()).getMessage());
        }
    }

    private Lobby removeMemberFromLobby(Lobby lobby, User user) {
        lobby.getMembers().removeIf(c -> c.equals(user));
        setTeam(lobby, user, null);
        LOGGER.info(LogMessage.LOBBY_LEFT, user.getUserName(), lobby.getUuid());

        if (!lobby.getMembers().isEmpty()) {
            if (lobby.getOwner().equals(user)) {
                // Will always be present because we checked the size.
                Optional<User> newOwner = lobby.getMembers().stream().filter(c -> !c.equals(user)).findFirst();
                newOwner.ifPresent(lobby::setOwner);
            }
        } else {
            lobbies.remove(lobby.getUuid());
            LOGGER.info(LogMessage.LOBBY_DELETED, lobby.getUuid());

            applicationEventPublisher.publishEvent(new LobbyRemoveEvent(this, lobby));
        }
        return lobby;
    }

    public Lobby kickMember(Lobby lobby, User user, User target) throws ApplicationException {
        isOwnerOrThrowException(user, lobby);

        LOGGER.info(LogMessage.LOBBY_KICKED, user.getUserName(), target.getUserName(), lobby.getUuid());
        return removeMember(lobby, target);
    }

    public Lobby inviteMember(Lobby lobby, String inviterUserName, User invited) throws ApplicationException {
        if (lobby.getMembers().contains(invited) || lobby.getInvited().get(invited) == InviteStatus.PENDING) {
            LOGGER.info(LogMessage.LOBBY_ALREADY_INVITED, invited.getUserName(), lobby.getUuid());
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.LOBBY_ALREADY_INVITED, MessageFormatter.format(LogMessage.LOBBY_ALREADY_INVITED, invited.getSummonerName(), lobby.getUuid()).getMessage());
        }
        if (getAllInvitedLobbiesByUser(invited).size() >= userMaxLobbyInvites) {
            LOGGER.info(LogMessage.LOBBY_INVITE_LIMIT_REACHED, invited.getUserName());
            throw new ApplicationException(HttpStatus.CONFLICT, ApplicationExceptionCode.LOBBY_INVITE_LIMIT_REACHED, MessageFormatter.format(LogMessage.LOBBY_INVITE_LIMIT_REACHED, invited.getSummonerName()).getMessage());
        }

        lobby.getInvited().put(invited, InviteStatus.PENDING);
        LOGGER.info(LogMessage.LOBBY_INVITED, inviterUserName, invited.getUserName(), lobby.getUuid());
        return lobby;
    }

    public Lobby acceptInvite(String lobbyUuid, User invited) throws ApplicationException {
        var lobby = getLobby(lobbyUuid).orElseThrow(() -> {
            LOGGER.info(LogMessage.LOBBY_NOT_FOUND, lobbyUuid);
            return new ApplicationException(HttpStatus.NOT_FOUND, ApplicationExceptionCode.LOBBY_NOT_FOUND, MessageFormatter.format(LogMessage.LOBBY_NOT_FOUND, lobbyUuid).getMessage());
        });

        if (lobby.getInvited().get(invited) == InviteStatus.PENDING) {
            if (lobby.getMembers().contains(invited)) {
                LOGGER.info(LogMessage.LOBBY_ALREADY_MEMBER, invited.getUserName(), lobbyUuid);
                throw new ApplicationException(HttpStatus.CONFLICT, ApplicationExceptionCode.LOBBY_ALREADY_MEMBER,
                        MessageFormatter.format(LogMessage.LOBBY_ALREADY_MEMBER, invited.getSummonerName(), lobbyUuid).getMessage());
            } else {
                if (lobby.getMembers().size() != lobby.getLobbyType().getTeamSize()
                        || lobby.isCustom() && lobby.getMembers().size() != (lobby.getLobbyType().getTeamSize() * 2)) {
                    lobby.getMembers().add(invited);

                    // Initialized on create
                    if (lobby.getTeam1().size() != lobby.getLobbyType().getTeamSize()) {
                        setTeam(lobby, invited, LobbyTeam.TEAM1);
                    } else {
                        setTeam(lobby, invited, LobbyTeam.TEAM2);
                    }

                    lobby.getInvited().replace(invited, InviteStatus.ACCEPTED);
                    LOGGER.info(LogMessage.LOBBY_INVITE_ACCEPTED, invited.getUserName(), lobbyUuid);
                    return lobby;
                }

                LOGGER.info(LogMessage.LOBBY_FULL, invited.getUserName(), lobbyUuid);
                throw new ApplicationException(HttpStatus.CONFLICT, ApplicationExceptionCode.LOBBY_FULL, MessageFormatter.format(LogMessage.LOBBY_FULL, invited.getSummonerName(), lobbyUuid).getMessage());
            }
        }

        LOGGER.info(LogMessage.LOBBY_INVITE_INVALID, invited.getUserName(), lobbyUuid);
        throw new ApplicationException(HttpStatus.FORBIDDEN, ApplicationExceptionCode.LOBBY_INVITE_INVALID, MessageFormatter.format(LogMessage.LOBBY_INVITE_INVALID, invited.getSummonerName(), lobbyUuid).getMessage());
    }

    public Lobby removeInviteInternal(Lobby lobby, User invited) {
        lobby.getInvited().replace(invited, InviteStatus.DENIED);
        LOGGER.info(LogMessage.LOBBY_INVITE_DENIED, invited.getUserName(), lobby.getUuid());
        return lobby;
    }

    public Lobby denyInvite(String lobbyUuid, User invited) throws ApplicationException {
        var lobby = getLobby(lobbyUuid).orElseThrow(() -> {
            LOGGER.info(LogMessage.LOBBY_NOT_FOUND, lobbyUuid);
            return new ApplicationException(HttpStatus.NOT_FOUND, ApplicationExceptionCode.LOBBY_NOT_FOUND, MessageFormatter.format(LogMessage.LOBBY_NOT_FOUND, lobbyUuid).getMessage());
        });

        if (lobby.getInvited().get(invited) == InviteStatus.PENDING) {
            if (lobby.getMembers().contains(invited)) {
                LOGGER.info(LogMessage.LOBBY_ALREADY_MEMBER, invited.getUserName(), lobbyUuid);
                throw new ApplicationException(HttpStatus.CONFLICT, ApplicationExceptionCode.LOBBY_ALREADY_MEMBER,
                        MessageFormatter.format(LogMessage.LOBBY_ALREADY_MEMBER, invited.getSummonerName(), lobbyUuid).getMessage());
            } else {
                lobby.getInvited().replace(invited, InviteStatus.DENIED);
                LOGGER.info(LogMessage.LOBBY_INVITE_DENIED, invited.getUserName(), lobbyUuid);
                return lobby;
            }
        } else {
            LOGGER.info(LogMessage.LOBBY_INVITE_INVALID, invited.getUserName(), lobbyUuid);
            throw new ApplicationException(HttpStatus.FORBIDDEN, ApplicationExceptionCode.LOBBY_INVITE_INVALID, MessageFormatter.format(LogMessage.LOBBY_INVITE_INVALID, invited.getSummonerName(), lobbyUuid).getMessage());
        }
    }

    public Optional<Lobby> getLobby(String lobbyUuid) {
        return Optional.ofNullable(lobbies.get(lobbyUuid));
    }

    public Optional<Lobby> getLobbyByUser(User user) {
        for (Lobby lobby : lobbies.values()) {
            if (lobby.getMembers().contains(user)) {
                return Optional.of(lobby);
            }
        }
        return Optional.empty();
    }

    public Optional<Lobby> getInvitedLobbyByUser(User user) {
        for (Lobby lobby : lobbies.values()) {
            if (lobby.getInvited().get(user) == InviteStatus.PENDING) {
                return Optional.of(lobby);
            }
        }
        return Optional.empty();
    }

    public List<Lobby> getAllInvitedLobbiesByUser(User user) {
        List<Lobby> invLobbies = new ArrayList<>();
        for (Lobby lobby : lobbies.values()) {
            if (lobby.getInvited().get(user) == InviteStatus.PENDING) {
                invLobbies.add(lobby);
            }
        }
        return invLobbies;
    }

    public void setTeam(Lobby lobby, User user, LobbyTeam team) {
        if (lobby.isCustom()) {
            Optional<User> teamUser = lobby.getMembers().stream().filter(c -> c.equals(user)).findFirst();
            teamUser.ifPresent(u -> u.setLobbyTeam(team));
        }
    }

    public boolean switchTeam(Lobby lobby, User user) {
        if (lobby.isCustom()) {
            Optional<User> teamUser = lobby.getMembers().stream().filter(c -> c.equals(user)).findFirst();
            if (teamUser.isPresent()) {
                if (teamUser.get().getLobbyTeam() == LobbyTeam.TEAM2 && lobby.getTeam1().size() <= lobby.getLobbyType().getTeamSize()) {
                    teamUser.get().setLobbyTeam(LobbyTeam.TEAM1);
                    return true;
                } else if (teamUser.get().getLobbyTeam() == LobbyTeam.TEAM1 && lobby.getTeam2().size() <= lobby.getLobbyType().getTeamSize()) {
                    teamUser.get().setLobbyTeam(LobbyTeam.TEAM2);
                    return true;
                }
            }
        }

        return false;
    }

    private void teamCheckTypeSwitch(Lobby lobby) {
        for (User user : lobby.getMembers()) {
            if (lobby.isCustom()) {
                if (lobby.getTeam1().size() != lobby.getLobbyType().getTeamSize()) {
                    user.setLobbyTeam(LobbyTeam.TEAM1);
                } else {
                    user.setLobbyTeam(LobbyTeam.TEAM2);
                }
            } else {
                user.setLobbyTeam(null);
            }
        }
    }
}

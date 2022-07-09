package de.jandev.ls4apiserver.model.websocket.lobby;

import com.fasterxml.jackson.annotation.JsonProperty;
import de.jandev.ls4apiserver.model.lobby.InviteStatus;
import de.jandev.ls4apiserver.model.lobby.Lobby;
import de.jandev.ls4apiserver.model.lobby.LobbyType;
import de.jandev.ls4apiserver.model.user.User;
import de.jandev.ls4apiserver.model.user.dto.UserPublicOut;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.time.LocalDateTime;
import java.util.*;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class LobbyMessageOut {

    private String uuid = UUID.randomUUID().toString();

    private LobbyType lobbyType;

    private List<UserPublicOut> members = new ArrayList<>();

    private Map<String, InviteStatus> invited = new HashMap<>();

    private UserPublicOut owner;

    private LocalDateTime joinedQueueTime;

    private boolean inQueue;

    @JsonProperty("isCustom")
    private boolean isCustom;

    public LobbyMessageOut(Lobby lobby) {
        this.uuid = lobby.getUuid();
        this.lobbyType = lobby.getLobbyType();
        this.owner = new UserPublicOut(lobby.getOwner());
        this.joinedQueueTime = lobby.getJoinedQueueTime();
        this.inQueue = lobby.isInQueue();
        this.isCustom = lobby.isCustom();

        for (User member : lobby.getMembers()) {
            this.members.add(new UserPublicOut(member));
        }

        for (Map.Entry<User, InviteStatus> member : lobby.getInvited().entrySet()) {
            this.invited.put(member.getKey().getSummonerName(), member.getValue());
        }
    }
}

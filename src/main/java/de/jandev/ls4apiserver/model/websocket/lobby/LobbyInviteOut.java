package de.jandev.ls4apiserver.model.websocket.lobby;

import de.jandev.ls4apiserver.model.user.dto.UserPublicOut;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class LobbyInviteOut {

    private String lobbyUuid;

    private UserPublicOut inviter;
}

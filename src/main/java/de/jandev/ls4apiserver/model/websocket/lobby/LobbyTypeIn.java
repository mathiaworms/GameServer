package de.jandev.ls4apiserver.model.websocket.lobby;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
public class LobbyTypeIn {

    private String lobbyType;

    @JsonProperty("isCustom")
    private boolean isCustom;
}

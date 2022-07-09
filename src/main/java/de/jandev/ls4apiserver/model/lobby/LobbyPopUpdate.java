package de.jandev.ls4apiserver.model.lobby;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class LobbyPopUpdate {

    private int accepted;
    private int pending;
    private int denied;
}

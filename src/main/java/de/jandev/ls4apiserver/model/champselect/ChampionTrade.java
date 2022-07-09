package de.jandev.ls4apiserver.model.champselect;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class ChampionTrade {

    private LobbyUser initiator;
    private LobbyUser target;
    private LobbyTeam lobbyTeam;
    private Integer timer;
}

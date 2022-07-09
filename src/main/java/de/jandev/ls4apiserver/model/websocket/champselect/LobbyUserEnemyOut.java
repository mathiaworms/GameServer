package de.jandev.ls4apiserver.model.websocket.champselect;

import de.jandev.ls4apiserver.model.champselect.LobbyUser;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class LobbyUserEnemyOut {

    private ChampionOut selectedChampion;

    private boolean lockedIn;

    public LobbyUserEnemyOut(LobbyUser lobbyUser, boolean withChampion) {
        if (withChampion && lobbyUser.getSelectedChampion() != null) {
            this.selectedChampion = new ChampionOut(lobbyUser.getSelectedChampion());
        }
        this.lockedIn = lobbyUser.isLockedIn();
    }
}

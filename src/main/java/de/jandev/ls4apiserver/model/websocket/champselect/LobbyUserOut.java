package de.jandev.ls4apiserver.model.websocket.champselect;

import de.jandev.ls4apiserver.model.champselect.LobbyTeam;
import de.jandev.ls4apiserver.model.champselect.LobbyUser;
import de.jandev.ls4apiserver.model.champselect.SummonerSpell;
import de.jandev.ls4apiserver.model.user.dto.UserPublicOut;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class LobbyUserOut {

    private UserPublicOut user;

    private ChampionOut selectedChampion;

    private SkinOut selectedSkin;

    private boolean lockedIn;

    private SummonerSpell spell1;

    private SummonerSpell spell2;

    private boolean visibleToEnemy;

    private boolean canLockIn;

    private boolean canBan;

    private LobbyTeam team;

    public LobbyUserOut(LobbyUser lobbyUser) {
        this.user = new UserPublicOut(lobbyUser.getUser());
        if (lobbyUser.getSelectedChampion() != null) {
            this.selectedChampion = new ChampionOut(lobbyUser.getSelectedChampion());
        }
        if (lobbyUser.getSelectedSkin() != null) {
            this.selectedSkin = new SkinOut(lobbyUser.getSelectedSkin());
        }
        this.lockedIn = lobbyUser.isLockedIn();
        this.spell1 = lobbyUser.getSpell1();
        this.spell2 = lobbyUser.getSpell2();
        this.visibleToEnemy = lobbyUser.isVisibleToEnemy();
        this.canLockIn = lobbyUser.isCanLockIn();
        this.canBan = lobbyUser.isCanBan();
        this.team = lobbyUser.getTeam();
    }
}

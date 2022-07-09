package de.jandev.ls4apiserver.model.champselect;

import de.jandev.ls4apiserver.model.collection.champion.Champion;
import de.jandev.ls4apiserver.model.collection.skin.Skin;
import de.jandev.ls4apiserver.model.user.User;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class LobbyUser {

    private User user;

    private Champion selectedChampion;

    private Skin selectedSkin;

    private boolean lockedIn;

    private SummonerSpell spell1 = SummonerSpell.SUMMONER_HEAL;

    private SummonerSpell spell2 = SummonerSpell.SUMMONER_FLASH;

    private boolean visibleToEnemy;

    private boolean canBan;

    private boolean canLockIn;

    private LobbyTeam team;

    public LobbyUser(User user) {
        this.user = user;
    }
}

package de.jandev.ls4apiserver.model.user.dto;

import de.jandev.ls4apiserver.model.champselect.LobbyTeam;
import de.jandev.ls4apiserver.model.user.SummonerStatus;
import de.jandev.ls4apiserver.model.user.User;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class UserPublicOut {

    private String summonerName;

    private int summonerIconId;

    private String summonerMotto;

    private SummonerStatus summonerStatus;

    private int summonerLevel;

    private boolean nameColourUnlocked;

    private LobbyTeam lobbyTeam;

    public UserPublicOut(User user) {
        this.summonerName = user.getSummonerName();
        this.summonerIconId = user.getSummonerIconId();
        this.summonerMotto = user.getSummonerMotto();
        this.summonerStatus = user.getSummonerStatus();
        this.summonerLevel = user.getSummonerLevel();
        this.lobbyTeam = user.getLobbyTeam();
        this.nameColourUnlocked = user.isNameColourUnlocked();
    }
}

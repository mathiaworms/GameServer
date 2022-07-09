package de.jandev.ls4apiserver.model.user.dto;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.fasterxml.jackson.datatype.jsr310.deser.LocalDateTimeDeserializer;
import com.fasterxml.jackson.datatype.jsr310.ser.LocalDateTimeSerializer;
import de.jandev.ls4apiserver.model.champselect.LobbyTeam;
import de.jandev.ls4apiserver.model.user.Role;
import de.jandev.ls4apiserver.model.user.SummonerStatus;
import de.jandev.ls4apiserver.model.user.User;
import lombok.Getter;
import lombok.Setter;

import java.time.LocalDateTime;
import java.util.List;

@Getter
@Setter
public class UserOut {

    private String uuid;

    private String userName;

    private String summonerName;

    private String email;

    private boolean emailConfirmed;

    private List<Role> roles;

    private String token;

    private int summonerIconId;

    private int summonerLevel;

    private int summonerExperienceNeeded;

    private int s4Coins;

    private boolean nameColourUnlocked;

    @JsonFormat(pattern = "yyyy-MM-dd HH:mm:ss")
    @JsonSerialize(using = LocalDateTimeSerializer.class)
    @JsonDeserialize(using = LocalDateTimeDeserializer.class)
    private LocalDateTime xpBoostUntil;

    private String summonerMotto;

    private SummonerStatus summonerStatus;

    private LobbyTeam lobbyTeam;

    public UserOut(User user) {
        this.uuid = user.getUuid();
        this.userName = user.getUserName();
        this.summonerName = user.getSummonerName();
        this.email = user.getEmail();
        this.emailConfirmed = user.isEmailConfirmed();
        this.roles = user.getRoles();
        this.token = user.getToken();
        this.summonerIconId = user.getSummonerIconId();
        this.summonerLevel = user.getSummonerLevel();
        this.summonerExperienceNeeded = user.getSummonerExperienceNeeded();
        this.s4Coins = user.getS4Coins();
        this.nameColourUnlocked = user.isNameColourUnlocked();
        this.xpBoostUntil = user.getXpBoostUntil();
        this.summonerMotto = user.getSummonerMotto();
        this.summonerStatus = user.getSummonerStatus();
        this.lobbyTeam = user.getLobbyTeam();
    }
}

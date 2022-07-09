package de.jandev.ls4apiserver.model.user;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.fasterxml.jackson.datatype.jsr310.deser.LocalDateTimeDeserializer;
import com.fasterxml.jackson.datatype.jsr310.ser.LocalDateTimeSerializer;
import de.jandev.ls4apiserver.model.champselect.LobbyTeam;
import de.jandev.ls4apiserver.model.collection.icon.Icon;
import de.jandev.ls4apiserver.model.collection.skin.Skin;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import javax.persistence.*;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Objects;

@Entity
@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class User {

    @Id
    private String uuid;

    @Column(unique = true, nullable = false)
    private String userName;

    @Column(unique = true, nullable = false)
    private String summonerName;

    @Column(unique = true, nullable = false)
    private String email;

    @JsonIgnore
    private String emailToken;

    private boolean emailConfirmed;

    @JsonIgnore
    @Column(nullable = false)
    private String password;

    @ElementCollection
    private List<Role> roles;

    @Transient
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

    @Enumerated(EnumType.STRING)
    private SummonerStatus summonerStatus;

    @OneToMany(mappedBy = "owner", cascade = CascadeType.ALL, orphanRemoval = true)
    @JsonIgnore // We ignore this because it will only be sent over websockets
    private List<Relationship> friends;

    @OneToMany(mappedBy = "friend", cascade = CascadeType.ALL, orphanRemoval = true)
    @JsonIgnore // We ignore this because it will only be sent over websockets
    private List<Relationship> friendedBy;

    @ManyToMany
    @JsonIgnore
    private List<Icon> ownedIcons;

    @ManyToMany
    @JsonIgnore
    private List<Skin> ownedSkins;

    @Transient
    private LobbyTeam lobbyTeam;

    @Override
    public String toString() {
        return "User{" +
                "uuid='" + uuid + '\'' +
                ", userName='" + userName + '\'' +
                ", summonerName='" + summonerName + '\'' +
                ", summonerLevel=" + summonerLevel +
                ", summonerExperienceNeeded=" + summonerExperienceNeeded +
                ", s4Coins=" + s4Coins +
                ", summonerMotto='" + summonerMotto + '\'' +
                ", summonerStatus=" + summonerStatus +
                '}';
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof User)) return false;
        var user = (User) o;
        return Objects.equals(uuid, user.uuid) && Objects.equals(userName, user.userName);
    }

    @Override
    public int hashCode() {
        return Objects.hash(uuid, userName);
    }
}
package de.jandev.ls4apiserver.model.champselect;

import de.jandev.ls4apiserver.model.collection.champion.Champion;
import de.jandev.ls4apiserver.model.lobby.LobbyType;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;
import java.util.UUID;
import java.util.stream.Collectors;
import java.util.stream.Stream;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class GameLobby {

    private String uuid = UUID.randomUUID().toString();

    private List<LobbyUser> team1 = new ArrayList<>();

    private List<LobbyUser> team2 = new ArrayList<>();

    private List<Champion> bansTeam1 = new ArrayList<>();

    private List<Champion> bansTeam2 = new ArrayList<>();

    private List<ChampionTrade> trades = new ArrayList<>();

    private LobbyType lobbyType;

    private int timer;

    private int maxBansTeam1;

    private int maxBansTeam2;

    private LobbyPhase lobbyPhase;

    public List<LobbyUser> getAllUsers() {
        return Stream.of(team1, team2).flatMap(Collection::stream).collect(Collectors.toList());
    }

    public List<Champion> getAllBans() {
        return Stream.of(bansTeam1, bansTeam2).flatMap(Collection::stream).collect(Collectors.toList());
    }

    public List<Champion> getAllPickedChampions() {
        return getAllUsers().stream().filter(LobbyUser::isLockedIn).map(LobbyUser::getSelectedChampion).collect(Collectors.toList());
    }

    public List<Champion> getAllPickedChampionsTeam(LobbyTeam team) {
        return getTeam(team).stream().filter(LobbyUser::isLockedIn).map(LobbyUser::getSelectedChampion).collect(Collectors.toList());
    }

    public int getAmountPickedTeam1() {
        return (int) getTeam1().stream().filter(LobbyUser::isLockedIn).count();
    }

    public int getAmountPickedTeam2() {
        return (int) getTeam2().stream().filter(LobbyUser::isLockedIn).count();
    }

    public List<LobbyUser> getTeam(LobbyTeam team) {
        if (team == LobbyTeam.TEAM1) {
            return team1;
        } else {
            return team2;
        }
    }
}

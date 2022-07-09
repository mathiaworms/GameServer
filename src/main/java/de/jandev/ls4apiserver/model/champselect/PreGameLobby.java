package de.jandev.ls4apiserver.model.champselect;

import de.jandev.ls4apiserver.model.lobby.Lobby;
import de.jandev.ls4apiserver.model.lobby.LobbyPopUpdate;
import de.jandev.ls4apiserver.model.lobby.LobbyType;
import de.jandev.ls4apiserver.model.user.User;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;
import java.util.stream.Collectors;
import java.util.stream.Stream;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class PreGameLobby {

    private List<Lobby> team1;

    private List<Lobby> team2;

    private Map<Lobby, List<User>> lobbyAmountAccepted = new ConcurrentHashMap<>();

    private Map<Lobby, List<User>> lobbyAmountDenied = new ConcurrentHashMap<>();

    private LobbyPopUpdate lobbyPopUpdate = new LobbyPopUpdate();

    private LobbyType lobbyType;

    public List<Lobby> getAllLobbies() {
        return Stream.of(team1, team2).flatMap(Collection::stream).collect(Collectors.toList());
    }

    public List<Lobby> getLobbiesNotAccepted() {
        List<Lobby> notAccepted = new ArrayList<>();
        for (Map.Entry<Lobby, List<User>> lobby : lobbyAmountAccepted.entrySet()) {
            if (lobby.getKey().getMembers().size() != lobby.getValue().size() || lobby.getKey().isAlteredDuringAccept()) {
                notAccepted.add(lobby.getKey());
            }
        }
        return notAccepted;
    }
}

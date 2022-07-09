package de.jandev.ls4apiserver.model.websocket.champselect;

import de.jandev.ls4apiserver.model.champselect.GameLobby;
import de.jandev.ls4apiserver.model.champselect.LobbyPhase;
import de.jandev.ls4apiserver.model.lobby.LobbyType;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class GameLobbyOut {

    private String uuid;

    private List<LobbyUserOut> team = new ArrayList<>();

    private List<LobbyUserEnemyOut> enemyTeam = new ArrayList<>();

    private List<ChampionOut> bansTeam1 = new ArrayList<>();

    private List<ChampionOut> bansTeam2 = new ArrayList<>();

    private List<ChampionTradeOut> tradesTeam = new ArrayList<>();

    private LobbyType lobbyType;

    private LobbyPhase lobbyPhase;

    private int timer;

    public GameLobbyOut(GameLobby gameLobby) {
        this.uuid = gameLobby.getUuid();
        this.bansTeam1 = gameLobby.getBansTeam1().stream().map(ChampionOut::new).collect(Collectors.toList());
        this.bansTeam2 = gameLobby.getBansTeam2().stream().map(ChampionOut::new).collect(Collectors.toList());
        this.lobbyType = gameLobby.getLobbyType();
        this.lobbyPhase = gameLobby.getLobbyPhase();
        this.timer = gameLobby.getTimer();
    }
}

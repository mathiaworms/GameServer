package de.jandev.ls4apiserver.model.websocket.champselect;

import de.jandev.ls4apiserver.model.champselect.ChampionTrade;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class ChampionTradeOut {

    private LobbyUserOut initiator;
    private LobbyUserOut target;
    private Integer timer;

    public ChampionTradeOut(ChampionTrade championTrade) {
        this.initiator = new LobbyUserOut(championTrade.getInitiator());
        this.target = new LobbyUserOut(championTrade.getTarget());
        this.timer = championTrade.getTimer();
    }
}

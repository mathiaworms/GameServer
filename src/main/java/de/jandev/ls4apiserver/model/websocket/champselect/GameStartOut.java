package de.jandev.ls4apiserver.model.websocket.champselect;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class GameStartOut {

    private String ip;

    private int port;

    private String blowfish;

    private int playerId;
}

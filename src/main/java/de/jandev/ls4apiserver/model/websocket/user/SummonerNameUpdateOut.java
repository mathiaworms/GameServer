package de.jandev.ls4apiserver.model.websocket.user;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class SummonerNameUpdateOut {

    private String oldSummonerName;

    private String newSummonerName;
}

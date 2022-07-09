package de.jandev.ls4apiserver.model.game;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class Player {

    private int playerId; // 1 - 10
    private String blowfishKey; // min. 16 byte
    private String rank; // DIAMOND
    private String name; // Player name
    private String champion; // Champion name
    private String team; // BLUE, RED
    private int skin; // Selected skin id
    private String summoner1; // SummonerHeal, SummonerFlash etc.
    private String summoner2;
    private int ribbon; // Selected ribbon id
    private int icon; // Selected icon id
    private Runes runes;
}

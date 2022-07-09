package de.jandev.ls4apiserver.model.game;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.util.List;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class Config {

    private Game game;
    private List<Player> players;
    private GameInfo gameInfo;
}

package de.jandev.ls4apiserver.model.game;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class Game {

    // [0] = typeof(FlatTestMap),
    // [1] = typeof(SummonersRift)
    // [2] = typeof(HarrowingRift),
    // [3] = typeof(ProvingGrounds),
    // [4] = typeof(TwistedTreeline),
    // [6] = typeof(WinterRift),
    // [8] = typeof(CrystalScar),
    // [10] = typeof(NewTwistedTreeline),
    // [11] = typeof(NewSummonersRift),
    // [12] = typeof(HowlingAbyss),
    // [14] = typeof(ButchersBridge)
    private int map;
    private String dataPackage; // LeagueSandbox-Scripts
    private String gameMode;
}

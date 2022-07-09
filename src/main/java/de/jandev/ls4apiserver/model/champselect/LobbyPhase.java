package de.jandev.ls4apiserver.model.champselect;

import lombok.Getter;

@Getter
public enum LobbyPhase {
    BAN_TEAM1(30), BAN_TEAM2(30), PICK_TEAM1(45), PICK_TEAM2(45), PICK_BLIND(90), PRE_START(30), PRE_START_ARAM(60);
    // Frontend logic limitation: Duration must always be different between phases.

    private final int duration;

    LobbyPhase(int duration) {
        this.duration = duration;
    }
}

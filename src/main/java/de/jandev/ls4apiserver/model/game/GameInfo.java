package de.jandev.ls4apiserver.model.game;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class GameInfo {

    @JsonProperty("MANACOSTS_ENABLED")
    private boolean manaCostsEnabled;
    @JsonProperty("COOLDOWNS_ENABLED")
    private boolean cooldownsEnabled;
    @JsonProperty("CHEATS_ENABLED")
    private boolean cheatsEnabled;
    @JsonProperty("MINION_SPAWNS_ENABLED")
    private boolean minionSpawnsEnabled;
    @JsonProperty("CONTENT_PATH")
    private String contentPath; // Path to game content folder. ../../../../../Content (Windows)  ../../../../Content (Linux) by default.
    @JsonProperty("IS_DAMAGE_TEXT_GLOBAL")
    private boolean isDamageTextGlobal;
}

package de.jandev.ls4apiserver.model.champselect;

import lombok.Getter;

@Getter
public enum SummonerSpell {
    SUMMONER_BARRIER("SummonerBarrier"), SUMMONER_BOOST("SummonerBoost"),
    SUMMONER_CLAIRVOYANCE("SummonerClairvoyance"), SUMMONER_DOT("SummonerDot"),
    SUMMONER_EXHAUST("SummonerExhaust"), SUMMONER_FLASH("SummonerFlash"),
    SUMMONER_HASTE("SummonerHaste"), SUMMONER_HEAL("SummonerHeal"),
    SUMMONER_MANA("SummonerMana"), SUMMONER_REVIVE("SummonerRevive"),
    SUMMONER_SMITE("SummonerSmite"), SUMMONER_TELEPORT("SummonerTeleport"),
    SUMMONER_ODIN_GARRISON("SummonerOdinGarrison");

    private final String gameServerName;

    SummonerSpell(String gameServerName) {
        this.gameServerName = gameServerName;
    }
}

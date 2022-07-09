package de.jandev.ls4apiserver.model.user.dto;

import de.jandev.ls4apiserver.model.collection.Availability;
import de.jandev.ls4apiserver.model.collection.champion.Champion;
import de.jandev.ls4apiserver.model.collection.champion.Spell;
import lombok.Getter;
import lombok.Setter;

import java.util.List;

@Setter
@Getter
public class UserChampionSkinOut {

    private String id;

    private String displayName;

    private String title;

    // Not native type so it can be null (if not purchasable)
    private Integer price;

    private Availability availability;

    private List<Spell> spells;

    private UserSkinsOut skins;

    public UserChampionSkinOut(Champion champion, UserSkinsOut skinsOut) {
        this.id = champion.getId();
        this.displayName = champion.getDisplayName();
        this.title = champion.getTitle();
        this.price = champion.getPrice();
        this.availability = champion.getAvailability();
        this.spells = champion.getSpells();
        this.skins = skinsOut;
    }
}

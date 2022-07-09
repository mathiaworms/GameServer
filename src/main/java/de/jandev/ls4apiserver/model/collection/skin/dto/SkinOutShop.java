package de.jandev.ls4apiserver.model.collection.skin.dto;

import de.jandev.ls4apiserver.model.collection.Availability;
import de.jandev.ls4apiserver.model.collection.skin.Skin;
import lombok.Getter;
import lombok.Setter;

import java.util.Objects;

@Setter
@Getter
public class SkinOutShop {

    private Long id;

    private String name;

    private int pictureId;

    private Integer price;

    private Availability availability;

    private String championId;

    public SkinOutShop(Skin skin) {
        this.id = skin.getId();
        this.name = skin.getName();
        this.pictureId = skin.getPictureId();
        this.price = skin.getPrice();
        this.availability = skin.getAvailability();
        this.championId = skin.getChampion().getId();
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof SkinOutShop)) return false;
        var skinOut = (SkinOutShop) o;
        return Objects.equals(id, skinOut.id);
    }

    @Override
    public int hashCode() {
        return Objects.hash(id);
    }
}

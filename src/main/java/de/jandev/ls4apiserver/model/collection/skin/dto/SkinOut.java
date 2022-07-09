package de.jandev.ls4apiserver.model.collection.skin.dto;

import com.fasterxml.jackson.annotation.JsonIgnore;
import de.jandev.ls4apiserver.model.collection.Availability;
import de.jandev.ls4apiserver.model.collection.champion.Champion;
import de.jandev.ls4apiserver.model.collection.skin.Skin;
import lombok.Getter;
import lombok.Setter;

import java.util.Objects;

@Setter
@Getter
public class SkinOut {

    private Long id;

    private String name;

    private int pictureId;

    private Integer price;

    private Availability availability;

    @JsonIgnore
    private Champion champion;

    public SkinOut(Skin skin) {
        this.id = skin.getId();
        this.name = skin.getName();
        this.pictureId = skin.getPictureId();
        this.price = skin.getPrice();
        this.availability = skin.getAvailability();
        this.champion = skin.getChampion();
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof SkinOut)) return false;
        var skinOut = (SkinOut) o;
        return Objects.equals(id, skinOut.id);
    }

    @Override
    public int hashCode() {
        return Objects.hash(id);
    }
}

package de.jandev.ls4apiserver.model.collection.icon.dto;

import de.jandev.ls4apiserver.model.collection.Availability;
import de.jandev.ls4apiserver.model.collection.icon.Icon;
import lombok.Getter;
import lombok.Setter;

import java.util.Objects;

@Setter
@Getter
public class IconOut {

    private int id;

    private Integer price;

    private Availability availability;

    public IconOut(Icon icon) {
        this.id = icon.getId();
        this.price = icon.getPrice();
        this.availability = icon.getAvailability();
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof IconOut)) return false;
        var iconOut = (IconOut) o;
        return id == iconOut.id;
    }

    @Override
    public int hashCode() {
        return Objects.hash(id);
    }
}

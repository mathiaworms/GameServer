package de.jandev.ls4apiserver.model.collection.icon;

import de.jandev.ls4apiserver.model.collection.Availability;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import javax.persistence.*;
import javax.validation.constraints.NotNull;
import java.util.Objects;

@Entity
@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class Icon {

    @Id
    @NotNull
    private int id;

    // Not native type so it can be null (if not purchasable)
    private Integer price;

    @Enumerated(EnumType.STRING)
    @Column(nullable = false)
    private Availability availability;

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof Icon)) return false;
        var icon = (Icon) o;
        return id == icon.id;
    }

    @Override
    public int hashCode() {
        return Objects.hash(id);
    }
}

package de.jandev.ls4apiserver.model.collection.skin;

import com.fasterxml.jackson.annotation.JsonIgnore;
import de.jandev.ls4apiserver.model.collection.Availability;
import de.jandev.ls4apiserver.model.collection.champion.Champion;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import javax.persistence.*;
import java.util.Objects;

@Entity
@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class Skin {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String name;

    private int pictureId;

    // Not native type so it can be null (if not purchasable)
    private Integer price;

    @Column(nullable = false)
    @Enumerated(EnumType.STRING)
    private Availability availability;

    @ManyToOne(optional = false, fetch = FetchType.LAZY)
    @JsonIgnore
    private Champion champion;

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof Skin)) return false;
        var skin = (Skin) o;
        return Objects.equals(id, skin.id);
    }

    @Override
    public int hashCode() {
        return Objects.hash(id);
    }
}

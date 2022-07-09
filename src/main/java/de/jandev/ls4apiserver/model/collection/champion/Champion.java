package de.jandev.ls4apiserver.model.collection.champion;

import de.jandev.ls4apiserver.model.collection.Availability;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import javax.persistence.*;
import javax.validation.constraints.NotNull;
import java.util.List;
import java.util.Objects;

@Entity
@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class Champion {

    @Id
    @NotNull
    private String id;

    @Column(nullable = false)
    private String displayName;

    @Column(nullable = false)
    private String title;

    // Not native type so it can be null (if not purchasable)
    private Integer price;

    @Enumerated(EnumType.STRING)
    @Column(nullable = false)
    private Availability availability;

    @OneToMany
    @JoinColumn(name = "champion_id", referencedColumnName = "id")
    private List<Spell> spells;

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof Champion)) return false;
        var champion = (Champion) o;
        return Objects.equals(id, champion.id);
    }

    @Override
    public int hashCode() {
        return Objects.hash(id);
    }
}

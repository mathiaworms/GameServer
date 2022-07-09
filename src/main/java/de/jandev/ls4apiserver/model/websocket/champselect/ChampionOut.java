package de.jandev.ls4apiserver.model.websocket.champselect;

import de.jandev.ls4apiserver.model.collection.champion.Champion;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.util.Objects;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class ChampionOut {

    private String id;

    public ChampionOut(Champion champion) {
        this.id = champion.getId();
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        ChampionOut that = (ChampionOut) o;
        return Objects.equals(id, that.id);
    }

    @Override
    public int hashCode() {
        return Objects.hash(id);
    }
}

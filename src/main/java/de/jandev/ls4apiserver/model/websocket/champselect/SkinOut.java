package de.jandev.ls4apiserver.model.websocket.champselect;

import de.jandev.ls4apiserver.model.collection.skin.Skin;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class SkinOut {

    private String name;

    public SkinOut(Skin skin) {
        this.name = skin.getName();
    }
}

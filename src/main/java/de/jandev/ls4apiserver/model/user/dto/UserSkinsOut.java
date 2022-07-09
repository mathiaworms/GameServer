package de.jandev.ls4apiserver.model.user.dto;

import de.jandev.ls4apiserver.model.collection.skin.dto.SkinOut;
import lombok.Getter;
import lombok.Setter;

import java.util.ArrayList;
import java.util.List;

@Getter
@Setter
public class UserSkinsOut {

    List<SkinOut> owned = new ArrayList<>();

    List<SkinOut> notOwned = new ArrayList<>();
}

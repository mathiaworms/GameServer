package de.jandev.ls4apiserver.model.shop;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class ShopItem {

    // Equals picture id. CDN: /shop/<id>
    private int id;

    private String name;

    private String description;

    private Integer price;
}

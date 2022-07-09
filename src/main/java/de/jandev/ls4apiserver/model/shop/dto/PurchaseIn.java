package de.jandev.ls4apiserver.model.shop.dto;

import lombok.Getter;
import lombok.Setter;

import javax.validation.constraints.NotBlank;
import javax.validation.constraints.NotNull;
import javax.validation.constraints.Pattern;
import javax.validation.constraints.Size;

@Getter
@Setter
public class PurchaseIn {

    @NotNull
    private Integer id;

    @NotNull
    private Integer expectedPrice;

    @NotBlank
    private String category;

    @Size(min = 3, max = 12)
    @Pattern(regexp = "^[a-zA-Z0-9]*$")
    private String summonerName;
}

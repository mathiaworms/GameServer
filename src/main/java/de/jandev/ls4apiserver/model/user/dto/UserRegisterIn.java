package de.jandev.ls4apiserver.model.user.dto;

import lombok.Getter;
import lombok.Setter;

import javax.validation.constraints.Email;
import javax.validation.constraints.NotNull;
import javax.validation.constraints.Pattern;
import javax.validation.constraints.Size;

@Getter
@Setter
public class UserRegisterIn {

    @NotNull
    @Email
    private String email;

    @NotNull
    @Size(min = 3, max = 12)
    @Pattern(regexp = "^[a-zA-Z0-9]*$")
    private String userName;

    @NotNull
    @Size(min = 3, max = 12)
    @Pattern(regexp = "^[a-zA-Z0-9]*$")
    private String summonerName;

    @NotNull
    @Size(min = 8, max = 128)
    private String password;
}

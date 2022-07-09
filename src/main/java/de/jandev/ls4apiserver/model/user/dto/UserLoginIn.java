package de.jandev.ls4apiserver.model.user.dto;

import lombok.Getter;
import lombok.Setter;

import javax.validation.constraints.NotNull;

@Getter
@Setter
public class UserLoginIn {

    @NotNull
    private String userName;

    @NotNull
    private String password;
}

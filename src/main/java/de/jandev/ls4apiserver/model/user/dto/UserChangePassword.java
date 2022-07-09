package de.jandev.ls4apiserver.model.user.dto;

import lombok.Getter;
import lombok.Setter;

import javax.validation.constraints.NotNull;
import javax.validation.constraints.Size;

@Getter
@Setter
public class UserChangePassword {

    @NotNull
    @Size(min = 8, max = 128)
    private String oldPassword;

    @NotNull
    @Size(min = 8, max = 128)
    private String newPassword;
}

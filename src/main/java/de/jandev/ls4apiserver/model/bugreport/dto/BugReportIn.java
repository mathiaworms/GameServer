package de.jandev.ls4apiserver.model.bugreport.dto;

import lombok.Getter;
import lombok.Setter;

import javax.validation.constraints.NotNull;
import javax.validation.constraints.Size;

@Getter
@Setter
public class BugReportIn {

    @NotNull
    @Size(min = 3)
    private String description;

    @NotNull
    @Size(min = 10)
    private String text;
}

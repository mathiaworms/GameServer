package de.jandev.ls4apiserver.model.news.dto;

import lombok.Getter;
import lombok.Setter;
import org.hibernate.validator.constraints.URL;

import javax.validation.constraints.NotBlank;

@Getter
@Setter
public class NewsIn {

    @NotBlank
    private String author;

    @NotBlank
    private String title;

    @NotBlank
    private String content;

    @NotBlank
    @URL
    private String imageUrl;
}

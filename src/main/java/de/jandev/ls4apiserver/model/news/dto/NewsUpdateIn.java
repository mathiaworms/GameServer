package de.jandev.ls4apiserver.model.news.dto;

import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
public class NewsUpdateIn {

    private String author;

    private String title;

    private String content;

    private String imageUrl;
}

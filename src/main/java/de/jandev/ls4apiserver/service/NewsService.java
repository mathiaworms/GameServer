package de.jandev.ls4apiserver.service;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.exception.ApplicationExceptionCode;
import de.jandev.ls4apiserver.model.news.News;
import de.jandev.ls4apiserver.model.news.dto.NewsIn;
import de.jandev.ls4apiserver.model.news.dto.NewsOut;
import de.jandev.ls4apiserver.model.news.dto.NewsUpdateIn;
import de.jandev.ls4apiserver.repo.NewsRepository;
import de.jandev.ls4apiserver.service.utility.LogMessage;
import org.slf4j.helpers.MessageFormatter;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.List;
import java.util.stream.Collectors;

@Service
public class NewsService {

    private final NewsRepository newsRepository;

    public NewsService(NewsRepository newsRepository) {
        this.newsRepository = newsRepository;
    }

    public List<NewsOut> getAllNews() {
        return newsRepository.findAll().stream().map(NewsOut::new).collect(Collectors.toList());
    }

    public NewsOut getNews(long newsId) throws ApplicationException {
        return new NewsOut(newsRepository.findById(newsId).orElseThrow(() ->
                new ApplicationException(HttpStatus.NOT_FOUND, ApplicationExceptionCode.NEWS_NOT_FOUND, MessageFormatter.format(LogMessage.NEWS_NOT_FOUND, newsId).getMessage())));
    }

    public NewsOut createNews(NewsIn newsIn) {
        var news = new News();
        news.setAuthor(newsIn.getAuthor());
        news.setTitle(newsIn.getTitle());
        news.setContent(newsIn.getContent());
        news.setImageUrl(newsIn.getImageUrl());

        // We don't check for existence as every news article could be "new".
        newsRepository.save(news);

        return new NewsOut(news);
    }

    public NewsOut updateNews(long newsId, NewsUpdateIn newsUpdateIn) throws ApplicationException {
        var news = newsRepository.findById(newsId).orElseThrow(() -> new ApplicationException(HttpStatus.NOT_FOUND, ApplicationExceptionCode.NEWS_NOT_FOUND, MessageFormatter.format(LogMessage.NEWS_NOT_FOUND, newsId).getMessage()));

        // We could use a mapper here, but as we don't need it anywhere else *yet* it's unnecessary.
        if (newsUpdateIn.getAuthor() != null) {
            news.setAuthor(newsUpdateIn.getAuthor());
        }

        if (newsUpdateIn.getTitle() != null) {
            news.setTitle(newsUpdateIn.getTitle());
        }

        if (newsUpdateIn.getContent() != null) {
            news.setContent(newsUpdateIn.getContent());
        }

        if (newsUpdateIn.getImageUrl() != null) {
            news.setImageUrl(newsUpdateIn.getImageUrl());
        }

        news.setUpdated(LocalDateTime.now());

        newsRepository.save(news);

        return new NewsOut(news);
    }

    public void deleteNews(long newsId) throws ApplicationException {
        var news = newsRepository.findById(newsId).orElseThrow(() -> new ApplicationException(HttpStatus.NOT_FOUND, ApplicationExceptionCode.NEWS_NOT_FOUND, MessageFormatter.format(LogMessage.NEWS_NOT_FOUND, newsId).getMessage()));

        newsRepository.delete(news);
    }
}

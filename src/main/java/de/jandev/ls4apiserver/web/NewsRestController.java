package de.jandev.ls4apiserver.web;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.model.news.dto.NewsIn;
import de.jandev.ls4apiserver.model.news.dto.NewsOut;
import de.jandev.ls4apiserver.model.news.dto.NewsUpdateIn;
import de.jandev.ls4apiserver.service.NewsService;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;

import javax.validation.Valid;
import java.util.List;

@RestController
@RequestMapping("/news")
public class NewsRestController implements BaseRestController {

    private final NewsService newsService;

    public NewsRestController(NewsService newsService) {
        this.newsService = newsService;
    }

    @PreAuthorize("hasAnyAuthority('USER','MODERATOR','ADMIN')")
    @GetMapping
    public List<NewsOut> getAllNews() {
        return newsService.getAllNews();
    }

    @PreAuthorize("hasAnyAuthority('USER','MODERATOR','ADMIN')")
    @GetMapping("/{newsId}")
    public NewsOut getNews(@PathVariable long newsId) throws ApplicationException {
        return newsService.getNews(newsId);
    }

    @PreAuthorize("hasAnyAuthority('MODERATOR','ADMIN')")
    @PutMapping
    public NewsOut createNews(@RequestBody @Valid NewsIn newsIn) {
        return newsService.createNews(newsIn);
    }

    @PreAuthorize("hasAnyAuthority('MODERATOR','ADMIN')")
    @PostMapping("/{newsId}")
    public NewsOut updateNews(@PathVariable long newsId, @RequestBody @Valid NewsUpdateIn newsUpdateIn) throws ApplicationException {
        return newsService.updateNews(newsId, newsUpdateIn);
    }

    @PreAuthorize("hasAnyAuthority('MODERATOR','ADMIN')")
    @DeleteMapping("/{newsId}")
    public void deleteNews(@PathVariable long newsId) throws ApplicationException {
        newsService.deleteNews(newsId);
    }
}

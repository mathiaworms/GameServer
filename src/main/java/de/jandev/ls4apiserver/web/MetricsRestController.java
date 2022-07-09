package de.jandev.ls4apiserver.web;

import de.jandev.ls4apiserver.service.GameManagerService;
import de.jandev.ls4apiserver.service.MatchmakingService;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/metrics")
public class MetricsRestController implements BaseRestController {

    private final MatchmakingService matchmakingService;
    private final GameManagerService gameManagerService;

    public MetricsRestController(MatchmakingService matchmakingService, GameManagerService gameManagerService) {
        this.matchmakingService = matchmakingService;
        this.gameManagerService = gameManagerService;
    }

    @PreAuthorize("hasAnyAuthority('USER','MODERATOR','ADMIN')")
    @GetMapping("/queue/count")
    public long getPlayersInQueue() {
        return matchmakingService.getQueue().stream().mapToLong(c -> c.getMembers().size()).sum();
    }

    @PreAuthorize("hasAnyAuthority('USER','MODERATOR','ADMIN')")
    @GetMapping("/queue/availableGameserver")
    public long getAvailableGameserver() {
        return gameManagerService.getServerList().size();
    }
}

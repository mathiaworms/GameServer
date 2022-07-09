package de.jandev.ls4apiserver.service.utility;

import de.jandev.ls4apiserver.model.champselect.GameLobby;
import de.jandev.ls4apiserver.model.champselect.LobbyPhase;
import de.jandev.ls4apiserver.model.event.GameLobbyFinalEvent;
import de.jandev.ls4apiserver.service.ChampselectService;
import org.springframework.context.ApplicationEventPublisher;

public class LobbyPhaseWorkerTask implements Runnable {

    private final ApplicationEventPublisher applicationEventPublisher;
    private final ChampselectService champselectService;
    private final GameLobby gameLobby;

    public LobbyPhaseWorkerTask(ApplicationEventPublisher applicationEventPublisher, ChampselectService champselectService, GameLobby gameLobby) {
        this.applicationEventPublisher = applicationEventPublisher;
        this.champselectService = champselectService;
        this.gameLobby = gameLobby;
    }

    @Override
    public void run() {
        if (gameLobby.getLobbyPhase() != LobbyPhase.PRE_START && gameLobby.getLobbyPhase() != LobbyPhase.PRE_START_ARAM) {
            applicationEventPublisher.publishEvent(new GameLobbyFinalEvent(this, gameLobby, true));
        } else {
            // Start the game
            champselectService.startNextPhase(gameLobby);
        }
    }
}

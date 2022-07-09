package de.jandev.ls4apiserver.service.utility;

import de.jandev.ls4apiserver.model.champselect.PreGameLobby;
import de.jandev.ls4apiserver.websocket.handler.LobbyMessageHandler;

public class QueuePopEndTask implements Runnable {

    private final PreGameLobby preGameLobby;
    private final LobbyMessageHandler lobbyMessageHandler;

    public QueuePopEndTask(PreGameLobby preGameLobby, LobbyMessageHandler lobbyMessageHandler) {
        this.preGameLobby = preGameLobby;
        this.lobbyMessageHandler = lobbyMessageHandler;
    }

    @Override
    public void run() {
        lobbyMessageHandler.handleQueuePopEndCallback(preGameLobby);
    }
}

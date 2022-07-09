package de.jandev.ls4apiserver.service;

import com.fasterxml.jackson.core.util.DefaultPrettyPrinter;
import com.fasterxml.jackson.databind.ObjectMapper;
import de.jandev.ls4apiserver.model.champselect.GameLobby;
import de.jandev.ls4apiserver.model.champselect.LobbyTeam;
import de.jandev.ls4apiserver.model.game.*;
import de.jandev.ls4apiserver.model.websocket.champselect.GameStartOut;
import de.jandev.ls4apiserver.service.utility.LogMessage;
import de.jandev.ls4apiserver.service.utility.ServerKillTask;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.scheduling.TaskScheduler;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Service;

import javax.annotation.PostConstruct;
import java.io.File;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.time.Instant;
import java.time.LocalDate;
import java.util.*;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ScheduledFuture;
import java.util.stream.Collectors;
import java.util.stream.Stream;

@Service
public class GameManagerService {
    // This service is temporary and will be replaced by the automatic pterodactyl server spin-up service later on
    // Issues are among others: Static blowfish keys, static ranks, static ribbons and static runes

    private static final Logger LOGGER = LoggerFactory.getLogger(GameManagerService.class);
    private static final String OS = System.getProperty("os.name", "generic").toLowerCase(Locale.ENGLISH);
    private final TaskScheduler taskScheduler;
    private final List<Integer> serverList = new ArrayList<>();
    private final Map<ServerKillTask, ScheduledFuture<?>> tasks = new ConcurrentHashMap<>();
    @Value("${game.server.path}")
    private String gameServerPath;
    @Value("${game.server.maxgametime}")
    private int maxGameTime;
    @Value("${game.server.ip}")
    private String gameServerIp;

    public GameManagerService(TaskScheduler taskScheduler) {
        this.taskScheduler = taskScheduler;
    }

    @PostConstruct
    public void fill() {
        for (var i = 1337; i <= 1348; i++) {
            serverList.add(i);
        }
    }

    @Scheduled(initialDelay = 5000, fixedDelay = 10000)
    private void recheckProcesses() {
        List<ServerKillTask> toRemove = new ArrayList<>();
        for (Map.Entry<ServerKillTask, ScheduledFuture<?>> entry : tasks.entrySet()) {
            if (!entry.getKey().getProcess().isAlive()) {
                entry.getValue().cancel(true);

                if (!serverList.contains(entry.getKey().getToAdd())) {
                    serverList.add(entry.getKey().getToAdd());
                }

                toRemove.add(entry.getKey());
            }
        }

        toRemove.forEach(tasks.keySet()::remove);
    }

    public List<GameStartOut> start(GameLobby gameLobby) {
        List<GameStartOut> gameStartOuts = new ArrayList<>();
        var objectMapper = new ObjectMapper();
        var objectWriter = objectMapper.writer(new DefaultPrettyPrinter());
        var config = getGameConfig(gameLobby);
        var settings = getGameSettings();
        var port = serverList.isEmpty() ? null : serverList.get(0);

        if (config == null || port == null) {
            // This happens when there is a problem with the path-finding and it will be already logged.
            return Collections.emptyList();
        }

//        try (var s = new ServerSocket(0)) {
//            port = s.getLocalPort();
//        } catch (IOException e) {
//            LOGGER.error(LogMessage.UNHANDLED_EXCEPTION, e);
//            return Collections.emptyList();
//        }

        var settingsDir = new File(gameServerPath + "/Settings");

        // This checks if the directory doesn't exist yet and if it doesn't exists if it could be created
        if (settingsDir.isDirectory() || settingsDir.mkdir()) {
            try {
                objectWriter.writeValue(new File(gameServerPath + "/Settings/GameInfo.json"), config);
                objectWriter.writeValue(new File(gameServerPath + "/Settings/GameServerSettings.json"), settings);
            } catch (IOException e) {
                LOGGER.error(LogMessage.UNHANDLED_EXCEPTION, e);
                return Collections.emptyList();
            }
        }

        try {
            /*
            This is a fun one.

            Starting the GameServer without cmd /c results it in being a subprocess to the java process.
            This would be fine, but the GameServer is using blocking stdout (text in console) which the java process is the parent of and needs to handle.
            So if we're not handling it, the buffer of those streams fill up and the GameServer will be 'blocked'.
            To fix this, we can detach the process with cmd /c and hide it with <nul 2<&1 or we need to empty the streams continually.

            We need to check the platform, because on Linux we cannot use cmd /c, but we can use >/dev/null 2>&1 there.
             */

            Process p;
            ProcessBuilder pB;
            if (OS.contains("win")) {
                pB = new ProcessBuilder(gameServerPath + "/GameServerConsole.exe", "--port", String.valueOf(port));
            } else {
                pB = new ProcessBuilder(gameServerPath + "/GameServerConsole", "--port", String.valueOf(port));
            }

            pB.directory(new File(gameServerPath));
            pB.redirectErrorStream(true);

            var logDir = new File(System.getProperty("user.dir") + "/logs");

            // This checks if the directory doesn't exist yet and if it doesn't exists if it could be created
            if (logDir.isDirectory() || logDir.mkdir()) {
                var log = new File(logDir + "/ls-gameserver_" + LocalDate.now() + ".log");

                pB.redirectOutput(ProcessBuilder.Redirect.appendTo(log));
                p = pB.start();

                serverList.remove(0);
                var serverKillTask = new ServerKillTask(p, serverList, port);
                tasks.put(serverKillTask, taskScheduler.schedule(serverKillTask, Instant.now().plusSeconds(maxGameTime * 60L)));
            } else {
                LOGGER.error(LogMessage.UNHANDLED_EXCEPTION + " EXCEPTION: Log directory for LS server could not be created.");
            }
        } catch (IOException e) {
            LOGGER.error(LogMessage.UNHANDLED_EXCEPTION, e);
            return Collections.emptyList();
        }

        for (Player player : config.getPlayers()) {
            var gameStartOut = new GameStartOut();
            gameStartOut.setIp(gameServerIp);
            gameStartOut.setPort(port);
            gameStartOut.setBlowfish(player.getBlowfishKey());
            gameStartOut.setPlayerId(player.getPlayerId());

            gameStartOuts.add(gameStartOut);
        }

        LOGGER.info(LogMessage.CHAMPSELECT_GAME_STARTED, gameLobby.getUuid(), port, maxGameTime);

        return gameStartOuts;
    }

    public List<Integer> getServerList() {
        return serverList;
    }

    private Config getGameConfig(GameLobby gameLobby) {
        var config = new Config();
        var game = new Game();
        var gameInfo = new GameInfo();
        List<Player> players = new ArrayList<>();

        game.setMap(gameLobby.getLobbyType().getMapType());
        game.setDataPackage("LeagueSandbox-Scripts");

        // Set to ARAM if map is ARAM
        if (gameLobby.getLobbyType().getMapType() == 12) {
            game.setGameMode("ARAM");
        } else {
            game.setGameMode("CLASSIC");
        }

        String contentPath = getContentPath();

        if (contentPath == null) {
            return null;
        }

        gameInfo.setManaCostsEnabled(true);
        gameInfo.setCooldownsEnabled(true);
        gameInfo.setCheatsEnabled(false);
        gameInfo.setMinionSpawnsEnabled(true);
        gameInfo.setContentPath(contentPath);
        gameInfo.setDamageTextGlobal(false);

        for (var i = 0; i < gameLobby.getAllUsers().size(); i++) {
            var lobbyUser = gameLobby.getAllUsers().get(i);
            var p = new Player();
            p.setPlayerId(i + 1);
            p.setBlowfishKey("17BLOhi6KZsTtldTsizvHg==");
            p.setRank("DIAMOND");
            p.setName(lobbyUser.getUser().getSummonerName());
            p.setChampion(lobbyUser.getSelectedChampion().getId());
            p.setTeam(lobbyUser.getTeam() == LobbyTeam.TEAM1 ? "BLUE" : "RED");
            if (lobbyUser.getSelectedSkin() != null) {
                p.setSkin(lobbyUser.getSelectedSkin().getPictureId());
            }
            p.setSummoner1(lobbyUser.getSpell1().getGameServerName());
            p.setSummoner2(lobbyUser.getSpell2().getGameServerName());
            p.setRibbon(2);
            p.setIcon(0);
            p.setRunes(new Runes());
            players.add(p);
        }

        config.setGame(game);
        config.setGameInfo(gameInfo);
        config.setPlayers(players);

        return config;
    }

    private Settings getGameSettings() {
        return new Settings(false);
    }

    private String getContentPath() {
        String result = null;
        var path = Path.of(gameServerPath);

        while (result == null) {
            if (path == null) {
                break;
            }

            try (Stream<Path> s = Files.walk(path, 1)) {
                List<Path> paths = s.filter(c -> c.toFile().isDirectory() && c.toFile().getName().equals("Content")).collect(Collectors.toList());

                if (paths.size() == 1) {
                    result = paths.get(0).toString();
                } else {
                    path = path.getParent();
                }
            } catch (IOException e) {
                LOGGER.error(LogMessage.UNHANDLED_EXCEPTION, e);
                return null;
            }
        }
        return result;
    }

}

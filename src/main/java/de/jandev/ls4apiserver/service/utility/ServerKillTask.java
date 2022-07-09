package de.jandev.ls4apiserver.service.utility;

import java.util.List;

public class ServerKillTask implements Runnable {

    private final Process process;
    private final List<Integer> serverList;
    private final int toAdd;

    public ServerKillTask(Process process, List<Integer> serverList, int toAdd) {
        this.process = process;
        this.serverList = serverList;
        this.toAdd = toAdd;
    }

    @Override
    public void run() {
        if (!serverList.contains(toAdd)) {
            serverList.add(toAdd);
        }

        process.destroy();
    }

    public int getToAdd() {
        return toAdd;
    }

    public Process getProcess() {
        return process;
    }
}

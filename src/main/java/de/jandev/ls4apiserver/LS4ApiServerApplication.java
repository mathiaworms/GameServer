package de.jandev.ls4apiserver;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.context.event.ApplicationEnvironmentPreparedEvent;
import org.springframework.context.ApplicationListener;
import org.springframework.scheduling.annotation.EnableScheduling;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;

@SpringBootApplication
@EnableScheduling
public class LS4ApiServerApplication {

    public static void main(String[] args) {
        var ideMode = System.getProperty("ideMode");
        var application = new SpringApplication(LS4ApiServerApplication.class);
        if (ideMode == null || ideMode.equals("false")) {
            addInitHooks(application);
        }
        application.run(args);
    }

    private static void addInitHooks(SpringApplication application) {
        application.addListeners((ApplicationListener<ApplicationEnvironmentPreparedEvent>) event -> {
            var from = LS4ApiServerApplication.class.getClassLoader().getResourceAsStream("exampleproperties.txt");
            var to = Path.of(System.getProperty("user.dir") + "/application-dev.properties");

            if (from != null && !to.toFile().exists()) {
                try {
                    Files.copy(from, to);
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }
        });
    }

}

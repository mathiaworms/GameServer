package de.jandev.ls4apiserver.configuration;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Configuration;
import org.springframework.messaging.simp.config.MessageBrokerRegistry;
import org.springframework.web.socket.config.annotation.EnableWebSocketMessageBroker;
import org.springframework.web.socket.config.annotation.StompEndpointRegistry;
import org.springframework.web.socket.config.annotation.WebSocketMessageBrokerConfigurer;

import java.util.List;

@Configuration
@EnableWebSocketMessageBroker
public class WebSocketConfig implements WebSocketMessageBrokerConfigurer {

    @Value("#{'${websocket.allowedorigins}'.split(',')}")
    private List<String> allowedOrigins;

    @Override
    public void registerStompEndpoints(StompEndpointRegistry registry) {
        if (allowedOrigins == null || allowedOrigins.isEmpty() || allowedOrigins.get(0).equals("*")) {
            registry.addEndpoint("/socket")
                    .setAllowedOriginPatterns("*");
        } else {
            registry.addEndpoint("/socket")
                    .setAllowedOriginPatterns(allowedOrigins.toArray(String[]::new));
        }
    }

    @Override
    public void configureMessageBroker(MessageBrokerRegistry registry) {
        // /queue/ just means the mapping is meant for a single user
        registry.setApplicationDestinationPrefixes("/out")
                .enableSimpleBroker("/queue/system", "/queue/lobby", "/queue/champselect");
    }
}

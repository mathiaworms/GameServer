package de.jandev.ls4apiserver.configuration;

import de.jandev.ls4apiserver.security.JwtTokenFilterConfigurer;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.autoconfigure.security.servlet.PathRequest;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Profile;
import org.springframework.security.config.annotation.method.configuration.EnableGlobalMethodSecurity;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configuration.WebSecurityConfigurerAdapter;
import org.springframework.security.config.http.SessionCreationPolicy;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.web.cors.CorsConfiguration;
import org.springframework.web.cors.CorsConfigurationSource;
import org.springframework.web.cors.UrlBasedCorsConfigurationSource;

import java.util.Collections;
import java.util.List;

@Profile("dev")
@Configuration
@EnableGlobalMethodSecurity(prePostEnabled = true)
public class DevWebSecurityConfig extends WebSecurityConfigurerAdapter {

    private final JwtTokenFilterConfigurer jwtTokenFilterConfigurer;
    @Value("#{'${websocket.allowedorigins}'.split(',')}")
    private List<String> allowedOrigins;

    public DevWebSecurityConfig(JwtTokenFilterConfigurer jwtTokenFilterConfigurer) {
        this.jwtTokenFilterConfigurer = jwtTokenFilterConfigurer;
    }

    @Override
    protected void configure(HttpSecurity http) throws Exception {
        http.csrf().disable();
        http.cors();

        http.sessionManagement().sessionCreationPolicy(SessionCreationPolicy.STATELESS);

        http.authorizeRequests()
                .requestMatchers(PathRequest.toStaticResources().atCommonLocations()).permitAll()
                .antMatchers("/users/register").permitAll()
                .antMatchers("/users/login").permitAll()
                .antMatchers("/users/confirm").permitAll()
                .antMatchers("/users/resendConfirm").permitAll()
                .antMatchers("/bugs").permitAll()
                .antMatchers("/v2/api-docs", "/swagger-resources", "/swagger-resources/**", "/configuration/**", "/swagger-ui.html", "/webjars/**").permitAll()
                .anyRequest().authenticated();

        http.apply(jwtTokenFilterConfigurer);
    }

    @Bean
    CorsConfigurationSource corsConfigurationSource() {
        var configuration = new CorsConfiguration();
        configuration.setAllowedHeaders(Collections.singletonList("*"));
        configuration.setAllowedMethods(Collections.singletonList("*"));
        configuration.setAllowCredentials(true);

        if (allowedOrigins == null || allowedOrigins.isEmpty() || allowedOrigins.get(0).equals("*")) {
            configuration.setAllowedOriginPatterns(Collections.singletonList("*"));
        } else {
            configuration.setAllowedOriginPatterns(allowedOrigins);
        }

        var source = new UrlBasedCorsConfigurationSource();
        source.registerCorsConfiguration("/**", configuration);
        return source;
    }

    @Bean
    public PasswordEncoder passwordEncoder() {
        return new BCryptPasswordEncoder(12);
    }
}

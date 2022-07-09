package de.jandev.ls4apiserver.security;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.exception.ApplicationExceptionCode;
import de.jandev.ls4apiserver.model.user.Role;
import de.jandev.ls4apiserver.repo.UserRepository;
import de.jandev.ls4apiserver.service.utility.LogMessage;
import io.jsonwebtoken.JwtException;
import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.SignatureAlgorithm;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.slf4j.helpers.MessageFormatter;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.HttpStatus;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

import javax.annotation.PostConstruct;
import javax.servlet.http.HttpServletRequest;
import java.util.Base64;
import java.util.Date;
import java.util.List;
import java.util.stream.Collectors;

@Component
public class JwtTokenProvider {

    private static final Logger LOGGER = LoggerFactory.getLogger(JwtTokenProvider.class);
    private final UserRepository userRepository;

    @Value("${jwt.secret}")
    private String secretKey;
    @Value("${jwt.validity}")
    private long validityInSeconds;

    public JwtTokenProvider(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    @PostConstruct
    protected void init() {
        secretKey = Base64.getEncoder().encodeToString(secretKey.getBytes());
    }

    public String createToken(String userName, List<Role> roles) {
        var claims = Jwts.claims().setSubject(userName);
        claims.put("auth", roles.stream().map(s -> new SimpleGrantedAuthority(s.getAuthority())).collect(Collectors.toList()));

        var now = new Date();
        // JWT needs to be converted to milliseconds.
        var validity = new Date(now.getTime() + (validityInSeconds * 1000));

        return Jwts.builder()
                .setClaims(claims)
                .setIssuedAt(now)
                .setExpiration(validity)
                .signWith(SignatureAlgorithm.HS256, secretKey)
                .compact();
    }

    @Transactional
    public Authentication getAuthentication(String token) throws ApplicationException {
        String userName = getUserName(token);
        final var user = userRepository.findByUserName(userName).orElseThrow(() -> {
            LOGGER.info(LogMessage.USER_NOT_FOUND, userName);
            return new ApplicationException(HttpStatus.NOT_FOUND, ApplicationExceptionCode.USER_NOT_FOUND, MessageFormatter.format(LogMessage.USER_NOT_FOUND, userName).getMessage());
        });

        // This sets the authentication name also for WebSockets!
        return new UsernamePasswordAuthenticationToken(user.getUserName(), null, user.getRoles());
    }

    private String getUserName(String token) {
        return Jwts.parser().setSigningKey(secretKey).parseClaimsJws(token).getBody().getSubject();
    }

    String resolveToken(HttpServletRequest req) {
        String bearerToken = req.getHeader("Authorization");
        if (bearerToken != null && bearerToken.startsWith("Bearer ")) {
            return bearerToken.substring(7);
        } else if (req.getParameter("token") != null) { // For websockets, which do not support headers.
            return req.getParameter("token");
        }

        return null;
    }

    boolean validateToken(String token) throws ApplicationException {
        try {
            Jwts.parser().setSigningKey(secretKey).parseClaimsJws(token);
            return true;
        } catch (JwtException | IllegalArgumentException e) {
            throw new ApplicationException(HttpStatus.UNAUTHORIZED, ApplicationExceptionCode.JWT_INVALID_OR_EXPIRED, MessageFormatter.format(LogMessage.JWT_INVALID_OR_EXPIRED, token).getMessage());
        }
    }
}

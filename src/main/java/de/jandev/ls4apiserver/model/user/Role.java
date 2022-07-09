package de.jandev.ls4apiserver.model.user;

import org.springframework.security.core.GrantedAuthority;

public enum Role implements GrantedAuthority {

    USER, MODERATOR, ADMIN, BANNED;

    public String getAuthority() {
        return name();
    }
}

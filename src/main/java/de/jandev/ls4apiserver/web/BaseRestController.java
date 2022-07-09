package de.jandev.ls4apiserver.web;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.exception.ApplicationExceptionCode;
import de.jandev.ls4apiserver.service.utility.LogMessage;
import org.slf4j.helpers.MessageFormatter;
import org.springframework.http.HttpStatus;
import org.springframework.security.core.context.SecurityContextHolder;

public interface BaseRestController {

    default void checkUserOwnsResource(String userName) throws ApplicationException {
        if (!(getAuthenticatedUserName().equals(userName))) {
            throw new ApplicationException(HttpStatus.FORBIDDEN, ApplicationExceptionCode.USER_NO_PERMISSION, MessageFormatter.format(LogMessage.USER_NO_PERMISSION, userName).getMessage());
        }
    }

    default String getAuthenticatedUserName() {
        return SecurityContextHolder.getContext().getAuthentication().getName();
    }
}

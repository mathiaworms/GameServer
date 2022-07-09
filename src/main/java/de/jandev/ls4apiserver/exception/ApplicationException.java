package de.jandev.ls4apiserver.exception;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import lombok.Getter;
import org.springframework.http.HttpStatus;

@Getter
@JsonIgnoreProperties({"cause", "stackTrace", "localizedMessage", "suppressed", "applicationExceptionCode", "httpStatus"})
public class ApplicationException extends Exception {

    private final HttpStatus httpStatus;
    private final ApplicationExceptionCode applicationExceptionCode;

    private final int code;
    private final String message;

    public ApplicationException(HttpStatus httpStatus, ApplicationExceptionCode applicationExceptionCode, String message) {
        this(httpStatus, applicationExceptionCode, message, null);
    }

    public ApplicationException(HttpStatus httpStatus, ApplicationExceptionCode applicationExceptionCode, String message, Throwable throwable) {
        super(message, throwable);
        this.httpStatus = httpStatus;
        this.applicationExceptionCode = applicationExceptionCode;
        this.code = applicationExceptionCode.getCode();
        this.message = message;
    }
}

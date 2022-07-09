package de.jandev.ls4apiserver.exception;

import de.jandev.ls4apiserver.service.utility.LogMessage;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.http.converter.HttpMessageNotReadableException;
import org.springframework.security.access.AccessDeniedException;
import org.springframework.validation.FieldError;
import org.springframework.web.HttpRequestMethodNotSupportedException;
import org.springframework.web.bind.MethodArgumentNotValidException;
import org.springframework.web.bind.annotation.ControllerAdvice;
import org.springframework.web.bind.annotation.ExceptionHandler;

@ControllerAdvice
public class ApplicationExceptionHandler {

    private static final Logger LOGGER = LoggerFactory.getLogger(ApplicationExceptionHandler.class);

    @ExceptionHandler(ApplicationException.class)
    protected ResponseEntity<Object> handleApplicationException(ApplicationException e) {
        return new ResponseEntity<>(e, e.getHttpStatus());
    }

    @ExceptionHandler(AccessDeniedException.class)
    protected ResponseEntity<Object> handleAccessDeniedException(AccessDeniedException e) {
        return new ResponseEntity<>(LogMessage.REQUEST_FORBIDDEN, HttpStatus.FORBIDDEN);
    }

    @ExceptionHandler(HttpMessageNotReadableException.class)
    protected ResponseEntity<Object> handleMessageNotReadableException(HttpMessageNotReadableException e) {
        return new ResponseEntity<>(LogMessage.REQUEST_NOT_READABLE, HttpStatus.BAD_REQUEST);
    }

    @ExceptionHandler(DataIntegrityViolationException.class)
    protected ResponseEntity<Object> handleDataIntegrityViolationException(DataIntegrityViolationException e) {
        return new ResponseEntity<>(LogMessage.REQUEST_NOT_READABLE, HttpStatus.BAD_REQUEST);
    }

    @ExceptionHandler(MethodArgumentNotValidException.class)
    protected ResponseEntity<Object> handleMethodArgumentNotValidException(MethodArgumentNotValidException e) {
        if (!e.getBindingResult().getFieldErrors().isEmpty()) {
            var sb = new StringBuilder();
            for (FieldError fieldError : e.getBindingResult().getFieldErrors()) {
                if (sb.indexOf(fieldError.getField()) == -1) {
                    sb.append(fieldError.getField());
                    sb.append(" ");
                }
            }
            return new ResponseEntity<>(new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.REQUEST_NOT_READABLE, LogMessage.REQUEST_NOT_READABLE + " Fields incorrect: " + sb.toString().trim()), HttpStatus.BAD_REQUEST);
        } else {
            return new ResponseEntity<>(LogMessage.REQUEST_NOT_READABLE, HttpStatus.BAD_REQUEST);
        }
    }

    @ExceptionHandler(HttpRequestMethodNotSupportedException.class)
    protected ResponseEntity<Object> handleHttpRequestMethodNotSupportedException(HttpRequestMethodNotSupportedException e) {
        return new ResponseEntity<>(LogMessage.REQUEST_NOT_ALLOWED, HttpStatus.METHOD_NOT_ALLOWED);
    }

    @ExceptionHandler(Exception.class)
    protected ResponseEntity<Object> handleException(Exception e) {
        LOGGER.error(LogMessage.UNHANDLED_EXCEPTION, e);
        return new ResponseEntity<>(LogMessage.UNHANDLED_EXCEPTION_OUT, HttpStatus.INTERNAL_SERVER_ERROR);
    }
}

package de.jandev.ls4apiserver.service;


import de.jandev.ls4apiserver.model.user.Email;
import de.jandev.ls4apiserver.service.utility.LogMessage;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.mail.MailException;
import org.springframework.mail.SimpleMailMessage;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.stereotype.Service;

@Service
public class MailService {

    private static final Logger LOGGER = LoggerFactory.getLogger(MailService.class);
    private final JavaMailSender mailSender;
    @Value("${mailing.from}")
    private String from;

    public MailService(JavaMailSender mailSender) {
        this.mailSender = mailSender;
    }

    public void sendMail(Email email) {
        var message = new SimpleMailMessage();
        message.setFrom(from);
        message.setTo(email.getTo());
        message.setSubject(email.getSubject());
        message.setText(email.getText());

        try {
            mailSender.send(message);
        } catch (MailException e) {
            LOGGER.warn(LogMessage.MAIL_ERROR, email.getTo(), email.getSubject(), e);
        }
    }

}

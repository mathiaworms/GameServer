package de.jandev.ls4apiserver.model.alert.dto;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.fasterxml.jackson.datatype.jsr310.deser.LocalDateTimeDeserializer;
import com.fasterxml.jackson.datatype.jsr310.ser.LocalDateTimeSerializer;
import de.jandev.ls4apiserver.model.alert.Alert;
import de.jandev.ls4apiserver.model.alert.AlertType;
import lombok.Getter;
import lombok.Setter;

import java.time.LocalDateTime;

@Getter
@Setter
public class AlertOut {

    private Long id;

    private String title;

    private String content;

    private AlertType alertType;

    @JsonFormat(pattern = "yyyy-MM-dd HH:mm:ss")
    @JsonSerialize(using = LocalDateTimeSerializer.class)
    @JsonDeserialize(using = LocalDateTimeDeserializer.class)
    private LocalDateTime startTime;

    @JsonFormat(pattern = "yyyy-MM-dd HH:mm:ss")
    @JsonSerialize(using = LocalDateTimeSerializer.class)
    @JsonDeserialize(using = LocalDateTimeDeserializer.class)
    private LocalDateTime endTime;

    public AlertOut(Alert alert) {
        this.id = alert.getId();
        this.title = alert.getTitle();
        this.content = alert.getContent();
        this.alertType = alert.getAlertType();
        this.startTime = alert.getStartTime();
        this.endTime = alert.getEndTime();
    }
}

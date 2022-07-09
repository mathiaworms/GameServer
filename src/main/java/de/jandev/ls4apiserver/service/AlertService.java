package de.jandev.ls4apiserver.service;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.exception.ApplicationExceptionCode;
import de.jandev.ls4apiserver.model.alert.Alert;
import de.jandev.ls4apiserver.model.alert.dto.AlertIn;
import de.jandev.ls4apiserver.model.alert.dto.AlertOut;
import de.jandev.ls4apiserver.repo.AlertRepository;
import de.jandev.ls4apiserver.service.utility.LogMessage;
import org.slf4j.helpers.MessageFormatter;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.stream.Collectors;

@Service
public class AlertService {

    private final AlertRepository alertRepository;

    public AlertService(AlertRepository alertRepository) {
        this.alertRepository = alertRepository;
    }

    public List<AlertOut> getAlerts() {
        return alertRepository.findAll().stream().map(AlertOut::new).collect(Collectors.toList());
    }

    public AlertOut createAlert(AlertIn alertIn) {
        var alert = new Alert();
        alert.setTitle(alertIn.getTitle());
        alert.setContent(alertIn.getTitle());
        alert.setAlertType(alertIn.getAlertType());
        alert.setEndTime(alertIn.getEndTime());

        // We don't check for existence as every news article could be "new".
        alertRepository.save(alert);

        return new AlertOut(alert);
    }

    public void deleteAlert(long alertId) throws ApplicationException {
        var alert = alertRepository.findById(alertId).orElseThrow(() -> new ApplicationException(HttpStatus.NOT_FOUND, ApplicationExceptionCode.ALERT_NOT_FOUND, MessageFormatter.format(LogMessage.ALERT_NOT_FOUND, alertId).getMessage()));

        alertRepository.delete(alert);
    }
}

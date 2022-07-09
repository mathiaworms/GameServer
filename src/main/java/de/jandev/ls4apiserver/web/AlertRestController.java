package de.jandev.ls4apiserver.web;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.model.alert.dto.AlertIn;
import de.jandev.ls4apiserver.model.alert.dto.AlertOut;
import de.jandev.ls4apiserver.service.AlertService;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;

import javax.validation.Valid;
import java.util.List;

@RestController
@RequestMapping("/alerts")
public class AlertRestController implements BaseRestController {

    private final AlertService alertService;

    public AlertRestController(AlertService alertService) {
        this.alertService = alertService;
    }

    @PreAuthorize("hasAnyAuthority('USER','MODERATOR','ADMIN')")
    @GetMapping
    public List<AlertOut> getAlerts() {
        return alertService.getAlerts();
    }

    @PreAuthorize("hasAnyAuthority('MODERATOR','ADMIN')")
    @PutMapping
    public AlertOut createAlert(@RequestBody @Valid AlertIn alertIn) {
        return alertService.createAlert(alertIn);
    }

    @PreAuthorize("hasAnyAuthority('MODERATOR','ADMIN')")
    @DeleteMapping("/{alertId}")
    public void deleteAlert(@PathVariable long alertId) throws ApplicationException {
        alertService.deleteAlert(alertId);
    }
}

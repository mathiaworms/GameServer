package de.jandev.ls4apiserver.service;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.exception.ApplicationExceptionCode;
import de.jandev.ls4apiserver.model.bugreport.BugReport;
import de.jandev.ls4apiserver.model.bugreport.dto.BugReportOut;
import de.jandev.ls4apiserver.repo.BugReportRepository;
import de.jandev.ls4apiserver.service.utility.LogMessage;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.slf4j.helpers.MessageFormatter;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.stream.Collectors;

@Service
public class BugReportService {

    private static final Logger LOGGER = LoggerFactory.getLogger(BugReportService.class);
    private final BugReportRepository bugReportRepository;

    public BugReportService(BugReportRepository bugReportRepository) {
        this.bugReportRepository = bugReportRepository;
    }

    public BugReportOut createBugReport(String userName, String description, String text) throws ApplicationException {
        if (bugReportRepository.countAllByUserName(userName) >= 50) {
            throw new ApplicationException(HttpStatus.I_AM_A_TEAPOT, ApplicationExceptionCode.BUGREPORT_TOO_MANY, MessageFormatter.format(LogMessage.BUGREPORT_TOO_MANY, userName).getMessage());
        }

        var bugReport = new BugReport();
        bugReport.setUserName(userName);
        bugReport.setDescription(description);
        bugReport.setText(text);
        bugReportRepository.save(bugReport);
        LOGGER.info(LogMessage.BUGREPORT_ADDED, userName, description);

        return new BugReportOut(bugReport);
    }

    public List<BugReportOut> getBugReports() {
        return bugReportRepository.findAll().stream().map(BugReportOut::new).collect(Collectors.toList());
    }
}

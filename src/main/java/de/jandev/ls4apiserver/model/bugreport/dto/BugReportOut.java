package de.jandev.ls4apiserver.model.bugreport.dto;

import de.jandev.ls4apiserver.model.bugreport.BugReport;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
public class BugReportOut {

    private Long id;

    private String userName;

    private String description;

    private String text;

    public BugReportOut(BugReport bugReport) {
        this.id = bugReport.getId();
        this.userName = bugReport.getUserName();
        this.description = bugReport.getDescription();
        this.text = bugReport.getText();
    }
}

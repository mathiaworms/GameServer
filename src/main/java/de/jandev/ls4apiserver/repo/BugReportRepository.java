package de.jandev.ls4apiserver.repo;

import de.jandev.ls4apiserver.model.bugreport.BugReport;
import org.springframework.data.jpa.repository.JpaRepository;

public interface BugReportRepository extends JpaRepository<BugReport, Long> {

    long countAllByUserName(String userName);
}

package de.jandev.ls4apiserver.repo;

import de.jandev.ls4apiserver.model.alert.Alert;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface AlertRepository extends JpaRepository<Alert, Long> {
}

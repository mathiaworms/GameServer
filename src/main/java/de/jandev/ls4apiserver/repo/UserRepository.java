package de.jandev.ls4apiserver.repo;

import de.jandev.ls4apiserver.model.user.SummonerStatus;
import de.jandev.ls4apiserver.model.user.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Modifying;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface UserRepository extends JpaRepository<User, String> {

    Optional<User> findBySummonerName(String summonerName);

    Optional<User> findByUserName(String userName);

    Optional<User> findByEmailToken(String token);

    Optional<User> findByEmail(String email);

    boolean existsByUserName(String userName);

    boolean existsBySummonerName(String summonerName);

    boolean existsByEmail(String email);

    @Modifying
    @Query("update User u set u.summonerStatus = :status")
    void setAllUserSummonerStatus(@Param(value = "status") SummonerStatus status);
}

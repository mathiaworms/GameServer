package de.jandev.ls4apiserver.repo;

import de.jandev.ls4apiserver.model.collection.Availability;
import de.jandev.ls4apiserver.model.collection.champion.Champion;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.jpa.repository.QueryHints;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import javax.persistence.QueryHint;
import java.util.List;
import java.util.Optional;

@Repository
public interface ChampionRepository extends JpaRepository<Champion, String> {

    @Query("select distinct champion from Champion champion LEFT JOIN FETCH champion.spells WHERE champion.availability <> 'LIMITED'")
    @QueryHints(value = {
            @QueryHint(name = "hibernate.query.passDistinctThrough", value = "false"),
    })
    List<Champion> findAllSpells();

    @Query("select champion from Champion champion LEFT JOIN FETCH champion.spells WHERE champion.id = :id AND champion.availability <> 'LIMITED'")
    Optional<Champion> findSpellsById(@Param("id") String id);

    Optional<Champion> findByIdAndAvailabilityNot(String id, Availability availability);
}

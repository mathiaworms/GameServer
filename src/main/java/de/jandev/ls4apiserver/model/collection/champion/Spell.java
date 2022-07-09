package de.jandev.ls4apiserver.model.collection.champion;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import javax.validation.constraints.NotNull;

@Entity
@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class Spell {

    @Id
    @NotNull
    private String id;

    @Column(nullable = false)
    private String displayName;
}

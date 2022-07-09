package de.jandev.ls4apiserver.model.user;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import javax.persistence.*;

@Entity
@AllArgsConstructor
@NoArgsConstructor
@Setter
@Getter
public class Relationship {

    @EmbeddedId
    private UserRelationshipKey id = new UserRelationshipKey();

    @ManyToOne
    @MapsId("ownerUuid")
    private User owner;

    @ManyToOne
    @MapsId("friendUuid")
    private User friend;

    @Enumerated(EnumType.STRING)
    private RelationshipStatus relationshipStatus;

    public Relationship(User owner, User friend, RelationshipStatus relationshipStatus) {
        this.owner = owner;
        this.friend = friend;
        this.relationshipStatus = relationshipStatus;
    }
}

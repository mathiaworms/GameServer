package de.jandev.ls4apiserver.web;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.model.user.dto.UserChampionSkinOut;
import de.jandev.ls4apiserver.model.user.dto.UserIconsOut;
import de.jandev.ls4apiserver.service.UserCollectionService;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/users/collection")
public class UserCollectionRestController implements BaseRestController {

    private final UserCollectionService userCollectionService;

    public UserCollectionRestController(UserCollectionService userCollectionService) {
        this.userCollectionService = userCollectionService;
    }

    /**
     * Gets all icons, separated by owned and not owned.
     *
     * @return all icons, separated by owned and not owned
     * @throws ApplicationException In case of a 404
     */
    @PreAuthorize("hasAnyAuthority('USER','MODERATOR','ADMIN')")
    @GetMapping("/icons")
    public UserIconsOut getIconsFromUser() throws ApplicationException {
        return userCollectionService.getIconsFromUser(getAuthenticatedUserName());
    }

    /**
     * Gets all champions with skins and spells
     *
     * @return all champions with skins and spells
     */
    @PreAuthorize("hasAnyAuthority('USER','MODERATOR','ADMIN')")
    @GetMapping("/champions")
    public List<UserChampionSkinOut> getChampions() throws ApplicationException {
        return userCollectionService.getChampions(getAuthenticatedUserName());
    }

    /**
     * Gets the champion with skins and spells filtered the id provided
     *
     * @param championId The id of the champion
     * @return user champion skin out object with skins and spells
     * @throws ApplicationException In case of a 404
     */
    @PreAuthorize("hasAnyAuthority('USER','MODERATOR','ADMIN')")
    @GetMapping("/champions/{championId}")
    public UserChampionSkinOut getChampion(@PathVariable String championId) throws ApplicationException {
        return userCollectionService.getChampionById(championId, getAuthenticatedUserName());
    }
}

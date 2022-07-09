package de.jandev.ls4apiserver.web;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.model.collection.skin.dto.SkinOutShop;
import de.jandev.ls4apiserver.model.shop.ShopItem;
import de.jandev.ls4apiserver.model.shop.dto.PurchaseIn;
import de.jandev.ls4apiserver.service.ShopService;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/shop")
public class ShopRestController implements BaseRestController {

    private final ShopService shopService;

    public ShopRestController(ShopService shopService) {
        this.shopService = shopService;
    }

    @PreAuthorize("hasAnyAuthority('USER','MODERATOR','ADMIN')")
    @GetMapping("/skins")
    public List<SkinOutShop> getSkins() throws ApplicationException {
        return shopService.getSkins(getAuthenticatedUserName());
    }

    @PreAuthorize("hasAnyAuthority('USER','MODERATOR','ADMIN')")
    @GetMapping("/icons")
    public List<ShopItem> getIcons() throws ApplicationException {
        return shopService.getIcons(getAuthenticatedUserName());
    }

    @PreAuthorize("hasAnyAuthority('USER','MODERATOR','ADMIN')")
    @GetMapping("/boosts")
    public List<ShopItem> getBoosts() {
        return shopService.getBoosts();
    }

    @PreAuthorize("hasAnyAuthority('USER','MODERATOR','ADMIN')")
    @GetMapping("/launcher")
    public List<ShopItem> getLauncher() throws ApplicationException {
        return shopService.getLauncher(getAuthenticatedUserName());
    }

    @PreAuthorize("hasAnyAuthority('USER','MODERATOR','ADMIN')")
    @GetMapping("/account")
    public List<ShopItem> getAccount() {
        return shopService.getAccount();
    }

    @PreAuthorize("hasAnyAuthority('USER','MODERATOR','ADMIN')")
    @PostMapping("/purchase")
    public void purchase(@RequestBody PurchaseIn purchaseIn) throws ApplicationException {
        shopService.purchase(getAuthenticatedUserName(), purchaseIn);
    }
}

package de.jandev.ls4apiserver.service;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.exception.ApplicationExceptionCode;
import de.jandev.ls4apiserver.model.collection.Availability;
import de.jandev.ls4apiserver.model.collection.skin.dto.SkinOutShop;
import de.jandev.ls4apiserver.model.shop.Category;
import de.jandev.ls4apiserver.model.shop.ShopItem;
import de.jandev.ls4apiserver.model.shop.dto.PurchaseIn;
import de.jandev.ls4apiserver.model.user.User;
import de.jandev.ls4apiserver.repo.IconRepository;
import de.jandev.ls4apiserver.repo.SkinRepository;
import de.jandev.ls4apiserver.repo.UserRepository;
import de.jandev.ls4apiserver.service.utility.LogMessage;
import de.jandev.ls4apiserver.websocket.GeneralSocketController;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.annotation.PostConstruct;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Optional;
import java.util.stream.Collectors;

@Service
public class ShopService {

    private final GeneralSocketController generalSocketController;
    private final UserService userService;
    private final IconRepository iconRepository;
    private final SkinRepository skinRepository;
    private final UserRepository userRepository;
    List<ShopItem> boosts = new ArrayList<>();
    List<ShopItem> launcher = new ArrayList<>();
    List<ShopItem> account = new ArrayList<>();

    public ShopService(UserService userService, IconRepository iconRepository, SkinRepository skinRepository, UserRepository userRepository, GeneralSocketController generalSocketController) {
        this.userService = userService;
        this.iconRepository = iconRepository;
        this.skinRepository = skinRepository;
        this.userRepository = userRepository;
        this.generalSocketController = generalSocketController;
    }

    @PostConstruct
    public void populateShop() {
        var xpBoost = new ShopItem();
        xpBoost.setId(0);
        xpBoost.setName("XP Boost 1");
        xpBoost.setDescription("1 Day");
        xpBoost.setPrice(100);

        var xpBoost2 = new ShopItem();
        xpBoost2.setId(1);
        xpBoost2.setName("XP Boost 2");
        xpBoost2.setDescription("3 Days");
        xpBoost2.setPrice(300);

        var xpBoost3 = new ShopItem();
        xpBoost3.setId(2);
        xpBoost3.setName("XP Boost 3");
        xpBoost3.setDescription("7 Days");
        xpBoost3.setPrice(700);

        boosts.add(xpBoost);
        boosts.add(xpBoost2);
        boosts.add(xpBoost3);

        var launcherCustomization = new ShopItem();
        launcherCustomization.setId(0);
        launcherCustomization.setName("Name Colour");
        launcherCustomization.setDescription("Gold");
        launcherCustomization.setPrice(300);

        launcher.add(launcherCustomization);

        var summonerNameChange = new ShopItem();
        summonerNameChange.setId(0);
        summonerNameChange.setName("Summoner Name Change");
        summonerNameChange.setPrice(1000);
        account.add(summonerNameChange);
    }

    @Transactional
    public void purchase(String userName, PurchaseIn purchaseIn) throws ApplicationException {
        Optional<Category> category = Arrays.stream(Category.values()).filter(c -> c.name().equalsIgnoreCase(purchaseIn.getCategory())).findFirst();

        if (category.isPresent()) {
            var user = userService.getUserByUserName(userName);

            switch (category.get()) {
                case SKINS:
                    purchaseSkin(user, purchaseIn);
                    break;
                case ICONS:
                    purchaseIcon(user, purchaseIn);
                    break;
                case BOOSTS:
                    purchaseBoost(user, purchaseIn);
                    break;
                case LAUNCHER:
                    purchaseLauncher(user, purchaseIn);
                    break;
                case ACCOUNT:
                    purchaseAccount(user, purchaseIn);
                    break;
                default:
                    break;
            }
        } else {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.REQUEST_NOT_READABLE, LogMessage.REQUEST_NOT_READABLE);
        }
    }

    @Transactional
    public void purchaseSkin(User user, PurchaseIn purchaseIn) throws ApplicationException {
        var skins = skinRepository.findAllByAvailabilityNot(Availability.LIMITED).stream()
                .filter(c -> c.getAvailability() == Availability.PURCHASABLE).collect(Collectors.toList());

        var skin = skins.stream().filter(c -> c.getId() == (long) purchaseIn.getId()).findFirst();

        if (skin.isPresent()
                && !user.getOwnedSkins().contains(skin.get())
                && user.getS4Coins() >= skin.get().getPrice()
                && purchaseIn.getExpectedPrice().equals(skin.get().getPrice())) {
            user.setS4Coins(user.getS4Coins() - skin.get().getPrice());
            user.getOwnedSkins().add(skin.get());
        } else {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.SHOP_NO_MONEY, LogMessage.SHOP_NO_MONEY);
        }
    }

    @Transactional
    public void purchaseIcon(User user, PurchaseIn purchaseIn) throws ApplicationException {
        var icons = iconRepository.findAllByAvailabilityNot(Availability.LIMITED).stream()
                .filter(c -> c.getAvailability() == Availability.PURCHASABLE).collect(Collectors.toList());

        var icon = icons.stream().filter(c -> c.getId() == purchaseIn.getId()).findFirst();

        if (icon.isPresent()
                && !user.getOwnedIcons().contains(icon.get())
                && user.getS4Coins() >= icon.get().getPrice()
                && purchaseIn.getExpectedPrice().equals(icon.get().getPrice())) {
            user.setS4Coins(user.getS4Coins() - icon.get().getPrice());
            user.getOwnedIcons().add(icon.get());
        } else {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.SHOP_NO_MONEY, LogMessage.SHOP_NO_MONEY);
        }
    }

    private void purchaseBoost(User user, PurchaseIn purchaseIn) throws ApplicationException {
        var shopItem = boosts.stream().filter(c -> c.getId() == purchaseIn.getId()).findFirst();

        if (shopItem.isPresent()
                && user.getS4Coins() >= shopItem.get().getPrice()
                && purchaseIn.getExpectedPrice().equals(shopItem.get().getPrice())) {
            var plusDays = 0;

            if (purchaseIn.getId() == 0) {
                plusDays = 1;
            } else if (purchaseIn.getId() == 1) {
                plusDays = 3;
            } else if (purchaseIn.getId() == 2) {
                plusDays = 7;
            }

            // Extend if xp boost is still running, otherwise now + plusDays
            if (user.getXpBoostUntil() != null && LocalDateTime.now().isBefore(user.getXpBoostUntil())) {
                user.setXpBoostUntil(user.getXpBoostUntil().plusDays(plusDays));
            } else {
                user.setXpBoostUntil(LocalDateTime.now().plusDays(plusDays));
            }

            user.setS4Coins(user.getS4Coins() - shopItem.get().getPrice());
            userRepository.save(user);
        } else {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.SHOP_NO_MONEY, LogMessage.SHOP_NO_MONEY);
        }
    }

    private void purchaseLauncher(User user, PurchaseIn purchaseIn) throws ApplicationException {
        var shopItem = launcher.stream().filter(c -> c.getId() == purchaseIn.getId()).findFirst();

        if (shopItem.isPresent()
                && user.getS4Coins() >= shopItem.get().getPrice()
                && purchaseIn.getId() == 0
                && purchaseIn.getExpectedPrice().equals(shopItem.get().getPrice())) {
            user.setNameColourUnlocked(true);

            user.setS4Coins(user.getS4Coins() - shopItem.get().getPrice());
            userRepository.save(user);
        } else {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.SHOP_NO_MONEY, LogMessage.SHOP_NO_MONEY);
        }
    }

    private void purchaseAccount(User user, PurchaseIn purchaseIn) throws ApplicationException {
        var shopItem = account.stream().filter(c -> c.getId() == purchaseIn.getId()).findFirst();

        if (shopItem.isPresent()
                && user.getS4Coins() >= shopItem.get().getPrice()
                && purchaseIn.getId() == 0
                && purchaseIn.getExpectedPrice().equals(shopItem.get().getPrice())) {
            var oldSummonerName = user.getSummonerName();
            user.setSummonerName(purchaseIn.getSummonerName());

            user.setS4Coins(user.getS4Coins() - shopItem.get().getPrice());
            userRepository.save(user);

            generalSocketController.forceDisconnectAndUpdateSummonerName(oldSummonerName, user);
        } else {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.SHOP_NO_MONEY, LogMessage.SHOP_NO_MONEY);
        }
    }

    @Transactional
    public List<SkinOutShop> getSkins(String userName) throws ApplicationException {
        var user = userService.getUserByUserName(userName);
        return skinRepository.findAllByAvailabilityNot(Availability.LIMITED).stream()
                .filter(c -> c.getAvailability() == Availability.PURCHASABLE && !user.getOwnedSkins().contains(c)).map(SkinOutShop::new).collect(Collectors.toList());
    }

    @Transactional
    public List<ShopItem> getIcons(String userName) throws ApplicationException {
        var user = userService.getUserByUserName(userName);

        return iconRepository.findAllByAvailabilityNot(Availability.LIMITED).stream()
                .filter(c -> c.getAvailability() == Availability.PURCHASABLE && !user.getOwnedIcons().contains(c)).map(c -> new ShopItem(c.getId(), null, null, c.getPrice())).collect(Collectors.toList());
    }

    public List<ShopItem> getBoosts() {
        return boosts;
    }

    public List<ShopItem> getLauncher(String userName) throws ApplicationException {
        var user = userService.getUserByUserName(userName);

        return launcher.stream().filter(c -> c.getId() != 0 || !user.isNameColourUnlocked()).collect(Collectors.toList());
    }

    public List<ShopItem> getAccount() {
        return account;
    }
}

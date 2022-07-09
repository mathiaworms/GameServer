package de.jandev.ls4apiserver.service;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.exception.ApplicationExceptionCode;
import de.jandev.ls4apiserver.model.collection.Availability;
import de.jandev.ls4apiserver.model.collection.champion.Champion;
import de.jandev.ls4apiserver.model.collection.icon.Icon;
import de.jandev.ls4apiserver.model.collection.icon.dto.IconOut;
import de.jandev.ls4apiserver.model.collection.skin.Skin;
import de.jandev.ls4apiserver.model.collection.skin.dto.SkinOut;
import de.jandev.ls4apiserver.model.user.dto.UserChampionSkinOut;
import de.jandev.ls4apiserver.model.user.dto.UserIconsOut;
import de.jandev.ls4apiserver.model.user.dto.UserSkinsOut;
import de.jandev.ls4apiserver.repo.ChampionRepository;
import de.jandev.ls4apiserver.repo.IconRepository;
import de.jandev.ls4apiserver.repo.SkinRepository;
import de.jandev.ls4apiserver.service.utility.LogMessage;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.slf4j.helpers.MessageFormatter;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;

@Service
@Transactional
public class UserCollectionService {

    private static final Logger LOGGER = LoggerFactory.getLogger(UserCollectionService.class);
    private final UserService userService;
    private final IconRepository iconRepository;
    private final ChampionRepository championRepository;
    private final SkinRepository skinRepository;

    public UserCollectionService(UserService userService, IconRepository iconRepository, ChampionRepository championRepository, SkinRepository skinRepository) {
        this.userService = userService;
        this.iconRepository = iconRepository;
        this.championRepository = championRepository;
        this.skinRepository = skinRepository;
    }

    public UserIconsOut getIconsFromUser(String userName) throws ApplicationException {
        var userIconsOut = new UserIconsOut();
        var user = userService.getUserByUserName(userName);
        List<Icon> icons = iconRepository.findAllByAvailabilityNot(Availability.LIMITED);

        userIconsOut.getOwned().addAll(user.getOwnedIcons().stream().map(IconOut::new).collect(Collectors.toList()));
        userIconsOut.getOwned().addAll(icons.stream().filter(c -> c.getAvailability() == Availability.PUBLIC).map(IconOut::new).collect(Collectors.toList()));
        userIconsOut.getNotOwned().addAll(icons.stream().filter(c -> !userIconsOut.getOwned().contains(new IconOut(c))).map(IconOut::new).collect(Collectors.toList()));

        return userIconsOut;
    }

    public UserSkinsOut getSkinsFromUser(String userName) throws ApplicationException {
        var userSkinsOut = new UserSkinsOut();
        var user = userService.getUserByUserName(userName);
        List<Skin> skins = skinRepository.findAllByAvailabilityNot(Availability.LIMITED);

        userSkinsOut.getOwned().addAll(skins.stream().filter(c -> c.getAvailability() == Availability.PUBLIC).map(SkinOut::new).collect(Collectors.toList()));
        userSkinsOut.getOwned().addAll(user.getOwnedSkins().stream().map(SkinOut::new).collect(Collectors.toList()));
        userSkinsOut.getNotOwned().addAll(skins.stream().filter(c -> !userSkinsOut.getOwned().contains(new SkinOut(c))).map(SkinOut::new).collect(Collectors.toList()));

        return userSkinsOut;
    }

    public List<Skin> getOwnedSkinsFromUser(String userName) throws ApplicationException {
        var user = userService.getUserByUserName(userName);
        List<Skin> skins = skinRepository.findAllByAvailabilityNot(Availability.LIMITED);
        List<Skin> skinsOut = new ArrayList<>();

        skinsOut.addAll(user.getOwnedSkins());
        skinsOut.addAll(skins.stream().filter(c -> c.getAvailability() == Availability.PUBLIC).collect(Collectors.toList()));

        return skinsOut;
    }

    public List<UserChampionSkinOut> getChampions(String userName) throws ApplicationException {
        var userSkinsOut = getSkinsFromUser(userName);
        List<UserChampionSkinOut> userChampionSkinOuts = new ArrayList<>();
        List<Champion> champions = championRepository.findAllSpells();

        for (Champion champion : champions) {
            var userSkinsOutFiltered = new UserSkinsOut();
            userSkinsOutFiltered.setOwned(userSkinsOut.getOwned().stream().filter(c -> c.getChampion().getId().equals(champion.getId())).collect(Collectors.toList()));
            userSkinsOutFiltered.setNotOwned(userSkinsOut.getNotOwned().stream().filter(c -> c.getChampion().getId().equals(champion.getId())).collect(Collectors.toList()));
            userChampionSkinOuts.add(new UserChampionSkinOut(champion, userSkinsOutFiltered));
        }

        return userChampionSkinOuts;
    }

    public UserChampionSkinOut getChampionById(String championId, String userName) throws ApplicationException {
        var userSkinsOut = getSkinsFromUser(userName);
        var userSkinsOutFiltered = new UserSkinsOut();
        userSkinsOutFiltered.setOwned(userSkinsOut.getOwned().stream().filter(c -> c.getChampion().getId().equals(championId)).collect(Collectors.toList()));
        userSkinsOutFiltered.setNotOwned(userSkinsOut.getNotOwned().stream().filter(c -> c.getChampion().getId().equals(championId)).collect(Collectors.toList()));

        var champion = championRepository.findSpellsById(championId).orElseThrow(() -> {
            LOGGER.info(LogMessage.CHAMPION_NOT_FOUND, championId);
            return new ApplicationException(HttpStatus.NOT_FOUND, ApplicationExceptionCode.CHAMPION_NOT_FOUND, MessageFormatter.format(LogMessage.CHAMPION_NOT_FOUND, championId).getMessage());
        });

        return new UserChampionSkinOut(champion, userSkinsOutFiltered);
    }

    public Champion getChampionByIdWithoutSpellsAndSkins(String championId) throws ApplicationException {
        return championRepository.findByIdAndAvailabilityNot(championId, Availability.LIMITED).orElseThrow(() -> {
            LOGGER.info(LogMessage.CHAMPION_NOT_FOUND, championId);
            return new ApplicationException(HttpStatus.NOT_FOUND, ApplicationExceptionCode.CHAMPION_NOT_FOUND, MessageFormatter.format(LogMessage.CHAMPION_NOT_FOUND, championId).getMessage());
        });
    }

    public List<Champion> getAllChampionsWithoutSpellsAndSkins() {
        return championRepository.findAll();
    }
}

package de.jandev.ls4apiserver.service;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.exception.ApplicationExceptionCode;
import de.jandev.ls4apiserver.model.collection.Availability;
import de.jandev.ls4apiserver.model.user.*;
import de.jandev.ls4apiserver.model.user.dto.UserChangePassword;
import de.jandev.ls4apiserver.model.user.dto.UserConfirmIn;
import de.jandev.ls4apiserver.model.user.dto.UserOut;
import de.jandev.ls4apiserver.model.user.dto.UserPublicOut;
import de.jandev.ls4apiserver.repo.IconRepository;
import de.jandev.ls4apiserver.repo.UserRepository;
import de.jandev.ls4apiserver.security.JwtTokenProvider;
import de.jandev.ls4apiserver.service.utility.LogMessage;
import org.hibernate.Hibernate;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.slf4j.helpers.MessageFormatter;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.HttpStatus;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.Arrays;
import java.util.Collections;
import java.util.Optional;
import java.util.UUID;

@Service
@Transactional
public class UserService {

    private static final Logger LOGGER = LoggerFactory.getLogger(UserService.class);
    private final MailService mailService;
    private final UserRepository userRepository;
    private final IconRepository iconRepository;
    private final JwtTokenProvider jwtTokenProvider;
    private final PasswordEncoder passwordEncoder;

    @Value("${mailing.enabled}")
    private boolean mailingEnabled;

    @Value("${mailing.confirm.link}")
    private String mailingConfirmLink;

    public UserService(MailService mailService, UserRepository userRepository, IconRepository iconRepository, JwtTokenProvider jwtTokenProvider, PasswordEncoder passwordEncoder) {
        this.mailService = mailService;
        this.userRepository = userRepository;
        this.iconRepository = iconRepository;
        this.jwtTokenProvider = jwtTokenProvider;
        this.passwordEncoder = passwordEncoder;
    }

    public UserOut register(String email, String userName, String summonerName, String password) throws ApplicationException {
        if (userRepository.existsByUserName(userName) || userRepository.existsBySummonerName(summonerName) || userRepository.existsByEmail(email)) {
            LOGGER.info(LogMessage.USER_ALREADY_EXISTS, email, summonerName, userName);
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.USER_ALREADY_EXISTS, MessageFormatter.arrayFormat(LogMessage.USER_ALREADY_EXISTS, new Object[]{email, summonerName, userName}).getMessage());
        }

        var uuid = UUID.randomUUID().toString();
        var user = new User();
        user.setUuid(uuid);
        user.setEmail(email);
        user.setUserName(userName);
        user.setSummonerName(summonerName);
        user.setPassword(passwordEncoder.encode(password));
        user.setRoles(Collections.singletonList(Role.USER));
        user.setSummonerStatus(SummonerStatus.OFFLINE);
        user.setSummonerLevel(1);
        user.setSummonerIconId(1);
        user.setSummonerMotto("");
        user.setSummonerExperienceNeeded(100);
        user.setS4Coins(500);
        userRepository.save(user);
        LOGGER.info(LogMessage.USER_CREATED, user.getUuid(), user.getSummonerName(), user.getUserName(), user.getEmail());

        // Cannot just use the user object. No clue why honestly. Throws a transaction cascading exception even though it shouldn't be the same transaction AFAIK.
        if (mailingEnabled) {
            // Don't set token here, as the user needs to confirm their email address first.

            sendConfirm(new UserConfirmIn(user.getEmail()));
        } else {
            user.setToken(jwtTokenProvider.createToken(user.getUserName(), user.getRoles()));
        }

        return new UserOut(user);
    }

    public void sendConfirm(UserConfirmIn userConfirmIn) throws ApplicationException {
        if (!mailingEnabled) {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.USER_MAILING_DISABLED, LogMessage.USER_MAILING_DISABLED);
        }

        var user = userRepository.findByEmail(userConfirmIn.getEmail()).orElseThrow(() -> {
            LOGGER.info(LogMessage.USER_NOT_FOUND_EMAIL, userConfirmIn.getEmail());
            return new ApplicationException(HttpStatus.NOT_FOUND, ApplicationExceptionCode.USER_NOT_FOUND_EMAIL, MessageFormatter.format(LogMessage.USER_NOT_FOUND_EMAIL, userConfirmIn.getEmail()).getMessage());
        });

        if (user.isEmailConfirmed()) {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.USER_ALREADY_CONFIRMED, LogMessage.USER_ALREADY_CONFIRMED);
        }

        String token = UUID.randomUUID().toString().replace("-", "");
        user.setEmailToken(token);

        var email = new Email();
        email.setTo(user.getEmail());
        email.setSubject("LeagueS4 - Confirm your email address");
        email.setText("Hey there!\n\nThank you for registering a new LeagueS4 account. Please confirm your email here:\n\n" + mailingConfirmLink + "/users/confirm?emailToken=" + token);

        mailService.sendMail(email);

        LOGGER.info(LogMessage.USER_REQUEST_CONFIRM, user.getUserName(), user.getEmail(), user.getEmailToken());
    }

    public void confirm(String token) throws ApplicationException {
        if (!mailingEnabled) {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.USER_MAILING_DISABLED, LogMessage.USER_MAILING_DISABLED);
        }

        var user = userRepository.findByEmailToken(token).orElseThrow(() -> {
            LOGGER.info(LogMessage.USER_NOT_FOUND_TOKEN, token);
            return new ApplicationException(HttpStatus.NOT_FOUND, ApplicationExceptionCode.USER_NOT_FOUND_TOKEN, MessageFormatter.format(LogMessage.USER_NOT_FOUND_TOKEN, token).getMessage());
        });

        if (user.isEmailConfirmed()) {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.USER_ALREADY_CONFIRMED, LogMessage.USER_ALREADY_CONFIRMED);
        }

        user.setEmailConfirmed(true);
        user.setEmailToken(null);

        LOGGER.info(LogMessage.USER_CONFIRM, user.getUserName(), user.getEmail(), token);
    }

    public UserOut login(String userName, String password) throws ApplicationException {
        var user = getUserByUserName(userName);

        // Check for equals instead of equalsIgnoreCase database call
        if (user.getUserName().equals(userName) && passwordEncoder.matches(password, user.getPassword())) {
            if (user.getRoles().contains(Role.BANNED)) {
                throw new ApplicationException(HttpStatus.UNAUTHORIZED, ApplicationExceptionCode.USER_BANNED, LogMessage.USER_BANNED);
            }

            if (mailingEnabled && !user.isEmailConfirmed()) {
                LOGGER.info(LogMessage.USER_LOGGED_IN_FAILED_EMAIL, user.getUserName());
                throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.USER_LOGGED_IN_FAILED_EMAIL, MessageFormatter.format(LogMessage.USER_LOGGED_IN_FAILED_EMAIL, user.getUserName()).getMessage());
            }

            String token = jwtTokenProvider.createToken(user.getUserName(), user.getRoles());
            user.setToken(token);
            LOGGER.info(LogMessage.USER_LOGGED_IN, user.getUserName(), token);
            return new UserOut(user);
        } else {
            LOGGER.info(LogMessage.USER_LOGGED_IN_FAILED, user.getUserName());
            throw new ApplicationException(HttpStatus.UNAUTHORIZED, ApplicationExceptionCode.USER_AUTH_INVALID, LogMessage.USER_AUTH_INVALID);
        }
    }

    public UserOut changePassword(String userName, UserChangePassword userChangePassword) throws ApplicationException {
        var user = getUserByUserName(userName);

        if (!passwordEncoder.matches(userChangePassword.getOldPassword(), user.getPassword())) {
            throw new ApplicationException(HttpStatus.UNAUTHORIZED, ApplicationExceptionCode.USER_AUTH_INVALID, LogMessage.USER_AUTH_INVALID);
        }

        user.setPassword(passwordEncoder.encode(userChangePassword.getNewPassword()));
        userRepository.save(user);
        LOGGER.info(LogMessage.USER_CHANGE_PASSWORD, user.getUuid(), user.getUserName());
        return new UserOut(user);
    }

    public User getUserByUserName(String userName) throws ApplicationException {
        var user = userRepository.findByUserName(userName).orElseThrow(() -> {
            LOGGER.info(LogMessage.USER_NOT_FOUND, userName);
            return new ApplicationException(HttpStatus.NOT_FOUND, ApplicationExceptionCode.USER_NOT_FOUND, MessageFormatter.format(LogMessage.USER_NOT_FOUND, userName).getMessage());
        });

        Hibernate.initialize(user.getRoles());

        return user;
    }

    public User getUserBySummonerName(String summonerName) throws ApplicationException {
        var user = userRepository.findBySummonerName(summonerName).orElseThrow(() -> {
            LOGGER.info(LogMessage.USER_NOT_FOUND, summonerName);
            return new ApplicationException(HttpStatus.NOT_FOUND, ApplicationExceptionCode.USER_NOT_FOUND, MessageFormatter.format(LogMessage.USER_NOT_FOUND, summonerName).getMessage());
        });

        Hibernate.initialize(user.getRoles());

        return user;
    }

    public UserOut getUserOut(String userName) throws ApplicationException {
        var user = userRepository.findByUserName(userName).orElseThrow(() -> {
            LOGGER.info(LogMessage.USER_NOT_FOUND, userName);
            return new ApplicationException(HttpStatus.NOT_FOUND, ApplicationExceptionCode.USER_NOT_FOUND, MessageFormatter.format(LogMessage.USER_NOT_FOUND, userName).getMessage());
        });

        Hibernate.initialize(user.getRoles());

        return new UserOut(user);
    }

    public UserPublicOut getUserPublicOut(String summonerName) throws ApplicationException {
        var user = userRepository.findBySummonerName(summonerName).orElseThrow(() -> {
            LOGGER.info(LogMessage.USER_NOT_FOUND, summonerName);
            return new ApplicationException(HttpStatus.NOT_FOUND, ApplicationExceptionCode.USER_NOT_FOUND, MessageFormatter.format(LogMessage.USER_NOT_FOUND, summonerName).getMessage());
        });

        Hibernate.initialize(user.getRoles());

        return new UserPublicOut(user);
    }

    public void changeAllUsersToOffline() {
        userRepository.setAllUserSummonerStatus(SummonerStatus.OFFLINE);
    }

    public void addFriend(User sender, User receiver) throws ApplicationException {
        checkSameUser(sender, receiver);

        if (sender.getFriends().stream().anyMatch(u -> u.getFriend().equals(receiver) && (u.getRelationshipStatus() == RelationshipStatus.PENDING || u.getRelationshipStatus() == RelationshipStatus.FRIEND))) {
            LOGGER.info(LogMessage.FRIEND_CONFLICT, receiver.getSummonerName());
            throw new ApplicationException(HttpStatus.CONFLICT, ApplicationExceptionCode.FRIEND_CONFLICT, MessageFormatter.format(LogMessage.FRIEND_CONFLICT, receiver.getSummonerName()).getMessage());
        }

        Optional<Relationship> senderFriendRequest = sender.getFriendedBy().stream().filter(c -> c.getOwner().equals(receiver)).findFirst();

        if (senderFriendRequest.isPresent()) {
            // This gets run when receiver is accepting it
            if (senderFriendRequest.get().getRelationshipStatus() == RelationshipStatus.PENDING) {
                sender.getFriends().add(new Relationship(sender, receiver, RelationshipStatus.FRIEND));
                senderFriendRequest.get().setRelationshipStatus(RelationshipStatus.FRIEND);
            } else if (senderFriendRequest.get().getRelationshipStatus() == RelationshipStatus.BLOCKED) {
                throw new ApplicationException(HttpStatus.FORBIDDEN, ApplicationExceptionCode.USER_FRIEND_ADD_BLOCKED, MessageFormatter.format(LogMessage.USER_FRIEND_ADD_BLOCKED, sender.getSummonerName(), receiver.getSummonerName()).getMessage());
            }
        } else {

            Optional<Relationship> senderFriend = sender.getFriends().stream().filter(c -> c.getFriend().equals(receiver)).findFirst();

            // IsPresent should always be blocked, because we already checked the other two cases above.
            if (senderFriend.isPresent() && senderFriend.get().getRelationshipStatus() == RelationshipStatus.BLOCKED) {
                senderFriend.get().setRelationshipStatus(RelationshipStatus.PENDING);
            } else {
                sender.getFriends().add(new Relationship(sender, receiver, RelationshipStatus.PENDING));
            }
        }

        LOGGER.info(LogMessage.USER_FRIEND_ADD, sender.getUserName(), receiver.getSummonerName());
    }

    public void removeFriend(User sender, User friend) throws ApplicationException {
        checkSameUser(sender, friend);

        // This allows a PENDING friend request to be removed, or a FRIEND to be removed
        // In case of BLOCKED, the friend cannot be removed. A friend cannot unfriend me when he got BLOCKED, because the block will already do that.
        if (sender.getFriends().stream().anyMatch(u -> u.getFriend().equals(friend) && u.getRelationshipStatus() != RelationshipStatus.BLOCKED)) {
            sender.getFriends().removeIf(c -> c.getFriend().equals(friend));
            friend.getFriends().removeIf(c -> c.getFriend().equals(sender));
        } else {
            LOGGER.info(LogMessage.FRIEND_NOT_FOUND, sender.getSummonerName());
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.FRIEND_NOT_FOUND, MessageFormatter.format(LogMessage.FRIEND_NOT_FOUND, sender.getSummonerName()).getMessage());
        }

        LOGGER.info(LogMessage.USER_FRIEND_REMOVE, sender.getUserName(), friend.getSummonerName());
    }

    public void denyFriendRequest(User receiver, User requester) throws ApplicationException {
        if (receiver.getFriendedBy().stream().anyMatch(u -> u.getOwner().equals(requester) && u.getRelationshipStatus() == RelationshipStatus.PENDING)) {
            requester.getFriends().removeIf(c -> c.getFriend().equals(receiver));
        } else {
            LOGGER.info(LogMessage.FRIEND_NOT_FOUND, receiver.getSummonerName());
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.FRIEND_NOT_FOUND, MessageFormatter.format(LogMessage.FRIEND_NOT_FOUND, receiver.getSummonerName()).getMessage());
        }

        LOGGER.info(LogMessage.USER_FRIEND_DECLINE, receiver.getUserName(), requester.getSummonerName());
    }

    public void blockFriend(User sender, User friend) throws ApplicationException {
        checkSameUser(sender, friend);

        // We don't use getActiveFriends here because we need the relationship
        Optional<Relationship> senderRelationship = sender.getFriends().stream().filter(c -> c.getFriend().equals(friend) && c.getRelationshipStatus() == RelationshipStatus.FRIEND).findFirst();

        if (senderRelationship.isPresent()) {
            senderRelationship.get().setRelationshipStatus(RelationshipStatus.BLOCKED);
            friend.getFriends().removeIf(c -> c.getFriend().equals(sender));
        } else {
            LOGGER.info(LogMessage.FRIEND_NOT_FOUND, sender.getSummonerName());
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.FRIEND_NOT_FOUND, MessageFormatter.format(LogMessage.FRIEND_NOT_FOUND, sender.getSummonerName()).getMessage());
        }

        LOGGER.info(LogMessage.USER_FRIEND_BLOCK, sender.getUserName(), friend.getSummonerName());
    }

    public void updateMotto(User user, String summonerMotto) throws ApplicationException {
        String mottoTrimmed = summonerMotto.trim();
        if (mottoTrimmed.length() < 1 || mottoTrimmed.length() > 30) {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.USER_MOTTO_TOO_LONG, LogMessage.USER_MOTTO_TOO_LONG);
        } else {
            user.setSummonerMotto(mottoTrimmed);
            LOGGER.info(LogMessage.MOTTO_UPDATED, user.getUserName(), mottoTrimmed);
        }
    }

    public void updateStatus(User user, String status) throws ApplicationException {
        Optional<SummonerStatus> summonerStatus = Arrays.stream(SummonerStatus.values()).filter(c -> c.name().equalsIgnoreCase(status)).findFirst();

        if (summonerStatus.isPresent()
                && (user.getSummonerStatus() == SummonerStatus.ONLINE || user.getSummonerStatus() == SummonerStatus.AWAY)
                && (summonerStatus.get() == SummonerStatus.ONLINE || summonerStatus.get() == SummonerStatus.AWAY)) {
            user.setSummonerStatus(summonerStatus.get());
            LOGGER.info(LogMessage.STATUS_UPDATED, user.getUserName(), summonerStatus.get());
        } else {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.USER_STATUS_INVALID, MessageFormatter.format(LogMessage.USER_STATUS_INVALID, status).getMessage());
        }
    }

    public void updateSummonerIcon(User user, Object sIcon) throws ApplicationException {
        if (!(sIcon instanceof Integer)) {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.ICON_INVALID, MessageFormatter.format(LogMessage.ICON_INVALID, sIcon).getMessage());
        }

        Integer summonerIcon = Integer.parseInt(String.valueOf(sIcon));

        var icon = iconRepository.findById(summonerIcon).orElseThrow(() -> {
            LOGGER.info(LogMessage.ICON_NOT_FOUND, summonerIcon);
            return new ApplicationException(HttpStatus.NOT_FOUND, ApplicationExceptionCode.ICON_NOT_FOUND, MessageFormatter.format(LogMessage.ICON_NOT_FOUND, summonerIcon).getMessage());
        });

        if (icon.getAvailability() != Availability.PUBLIC && !user.getOwnedIcons().contains(icon)) {
            throw new ApplicationException(HttpStatus.FORBIDDEN, ApplicationExceptionCode.ICON_NOT_OWNED, MessageFormatter.format(LogMessage.ICON_NOT_OWNED, summonerIcon).getMessage());
        }

        user.setSummonerIconId(summonerIcon);
        LOGGER.info(LogMessage.ICON_UPDATED, user.getUserName(), summonerIcon);
    }

    public User banUserBySummonerName(User admin, String summonerName) throws ApplicationException {
        if (!admin.getRoles().contains(Role.ADMIN) || admin.getSummonerName().equalsIgnoreCase(summonerName)) {
            throw new ApplicationException(HttpStatus.FORBIDDEN, ApplicationExceptionCode.USER_NO_PERMISSION, MessageFormatter.format(LogMessage.USER_NO_PERMISSION, admin.getUserName()).getMessage());
        }

        var user = userRepository.findBySummonerName(summonerName).orElseThrow(() -> {
            LOGGER.info(LogMessage.USER_NOT_FOUND, summonerName);
            return new ApplicationException(HttpStatus.NOT_FOUND, ApplicationExceptionCode.USER_NOT_FOUND, MessageFormatter.format(LogMessage.USER_NOT_FOUND, summonerName).getMessage());
        });

        Hibernate.initialize(user.getRoles());

        user.setRoles(Collections.singletonList(Role.BANNED));

        return user;
    }

    private void checkSameUser(User sender, User receiver) throws ApplicationException {
        if (sender.getSummonerName().equalsIgnoreCase(receiver.getSummonerName())) {
            LOGGER.info(LogMessage.FRIEND_SAME_USER, sender.getSummonerName());
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.FRIEND_SAME_USER, MessageFormatter.format(LogMessage.FRIEND_SAME_USER, sender.getSummonerName()).getMessage());
        }
    }
}

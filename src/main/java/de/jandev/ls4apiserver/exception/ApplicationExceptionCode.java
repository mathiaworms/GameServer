package de.jandev.ls4apiserver.exception;

import lombok.Getter;

@Getter
public enum ApplicationExceptionCode {
    UNHANDLED_EXCEPTION(0),
    JWT_INVALID_OR_EXPIRED(100),
    USER_AUTH_INVALID(101),
    USER_NOT_FOUND(102),
    USER_NOT_FOUND_TOKEN(103),
    USER_NOT_FOUND_EMAIL(104),
    USER_ALREADY_CONFIRMED(105),
    USER_MAILING_DISABLED(106),
    USER_NO_PERMISSION(107),
    USER_ALREADY_EXISTS(108),
    USER_FRIEND_ADD_BLOCKED(109),
    USER_LOGGED_IN_FAILED_EMAIL(110),
    USER_MOTTO_TOO_LONG(111),
    USER_STATUS_INVALID(112),
    FRIEND_CONFLICT(113),
    FRIEND_SAME_USER(114),
    FRIEND_NOT_FOUND(115),
    FRIEND_LIMIT_EXCEEDED(116),
    FRIEND_LIMIT_EXCEEDED_TARGET(117),
    FRIEND_REQUEST_LIMIT_EXCEEDED(118),
    FRIEND_REQUEST_LIMIT_EXCEEDED_TARGET(119),
    ICON_NOT_FOUND(120),
    ICON_INVALID(121),
    ICON_NOT_OWNED(122),
    CHAMPION_NOT_FOUND(123),
    LOBBY_INVITE_INVALID(124),
    LOBBY_INVITE_LIMIT_REACHED(125),
    LOBBY_FULL(126),
    LOBBY_ALREADY_MEMBER(127),
    LOBBY_ALREADY_INVITED(128),
    LOBBY_NOT_FOUND(129),
    LOBBY_NO_MEMBER(130),
    LOBBY_NO_OWNER(131),
    LOBBY_TYPE_INVALID(132),
    LOBBY_UNAUTHORIZED(133),
    LOBBY_MATCHMAKING_ACCEPT_IMPOSSIBLE(134),
    LOBBY_SWITCH_IMPOSSIBLE(135),
    LOBBY_IN_QUEUE_CANNOT_BE_JOINED(136),
    MATCHMAKING_ALREADY_IN_QUEUE(137),
    MATCHMAKING_NOT_IN_QUEUE(138),
    MATCHMAKING_BLOCKED_JOIN(139),
    MATCHMAKING_BLOCKED_JOIN_DODGE(140),
    MATCHMAKING_BLOCKED_LEAVE(141),
    CHAMPSELECT_INVALID_SPELL(142),
    CHAMPSELECT_INVALID_SKIN(143),
    CHAMPSELECT_BAN_FORBIDDEN(144),
    CHAMPSELECT_PICK_FORBIDDEN(145),
    CHAMPSELECT_MESSAGE_FORBIDDEN(146),
    BUGREPORT_TOO_MANY(147),
    CHAT_MESSAGE_TOO_LONG(148),
    REQUEST_NOT_READABLE(149),
    CHAMPSELECT_USER_NOT_FOUND(150),
    USER_LOGGED_IN_ANOTHER_LOCATION(151),
    NEWS_NOT_FOUND(152),
    USER_BANNED(153),
    CHAMPSELECT_TRADE_NOT_POSSIBLE(154),
    ALERT_NOT_FOUND(155),
    SHOP_NO_MONEY(156);

    private final int code;

    ApplicationExceptionCode(int code) {
        this.code = code;
    }
}
package de.jandev.ls4apiserver.service;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.exception.ApplicationExceptionCode;
import de.jandev.ls4apiserver.model.chat.ChatMessage;
import de.jandev.ls4apiserver.model.user.RelationshipStatus;
import de.jandev.ls4apiserver.model.user.User;
import de.jandev.ls4apiserver.repo.ChatRepository;
import de.jandev.ls4apiserver.service.utility.LogMessage;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.slf4j.helpers.MessageFormatter;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
public class ChatService {

    private static final Logger LOGGER = LoggerFactory.getLogger(ChatService.class);
    private final ChatRepository chatRepository;

    public ChatService(ChatRepository chatRepository) {
        this.chatRepository = chatRepository;
    }


    public List<ChatMessage> getChatMessagesWithFriend(User user, User friend) {
        return chatRepository.findFirst100ByFromAndToOrFromAndToOrderByMessageTimestampDesc(user, friend, friend, user);
    }

    public void createChatMessage(User from, User to, String message) throws ApplicationException {
        if (from.getFriends().stream().noneMatch(c -> c.getFriend().equals(to) && c.getRelationshipStatus() == RelationshipStatus.FRIEND)) {
            throw new ApplicationException(HttpStatus.NOT_FOUND, ApplicationExceptionCode.USER_NOT_FOUND, MessageFormatter.format(LogMessage.USER_NOT_FOUND, to.getSummonerName()).getMessage());
        }

        var chatMessage = new ChatMessage();

        chatMessage.setFrom(from);
        chatMessage.setTo(to);

        if (message.length() > 255) {
            throw new ApplicationException(HttpStatus.BAD_REQUEST, ApplicationExceptionCode.CHAT_MESSAGE_TOO_LONG, "Chat message too long.");
        } else {
            chatMessage.setData(message);
        }

        chatRepository.save(chatMessage);

        LOGGER.info(LogMessage.CHAT_PRIVATE_CREATED, from.getUserName(), to.getUserName(), message);
    }
}

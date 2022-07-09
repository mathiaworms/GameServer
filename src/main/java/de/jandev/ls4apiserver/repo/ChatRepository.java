package de.jandev.ls4apiserver.repo;

import de.jandev.ls4apiserver.model.chat.ChatMessage;
import de.jandev.ls4apiserver.model.user.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface ChatRepository extends JpaRepository<ChatMessage, Long> {

    List<ChatMessage> findFirst100ByFromAndToOrFromAndToOrderByMessageTimestampDesc(User from, User to, User to2, User from2);
}

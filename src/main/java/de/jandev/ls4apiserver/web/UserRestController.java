package de.jandev.ls4apiserver.web;

import de.jandev.ls4apiserver.exception.ApplicationException;
import de.jandev.ls4apiserver.model.user.dto.*;
import de.jandev.ls4apiserver.service.UserService;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;

import javax.servlet.http.HttpServletRequest;
import javax.validation.Valid;

@RestController
@RequestMapping("/users")
public class UserRestController implements BaseRestController {

    private final UserService userService;

    public UserRestController(UserService userService) {
        this.userService = userService;
    }

    @PreAuthorize("hasAnyAuthority('USER','MODERATOR','ADMIN')")
    @GetMapping
    public UserOut getUserSelf() throws ApplicationException {
        return userService.getUserOut(getAuthenticatedUserName());
    }

    @PutMapping("/register")
    public UserOut register(@RequestBody @Valid UserRegisterIn body) throws ApplicationException {
        return userService.register(body.getEmail(), body.getUserName(), body.getSummonerName(), body.getPassword());
    }

    @PostMapping("/login")
    public UserOut login(@RequestBody @Valid UserLoginIn body) throws ApplicationException {
        return userService.login(body.getUserName(), body.getPassword());
    }

    @PostMapping("/password")
    public UserOut changePassword(@RequestBody @Valid UserChangePassword body) throws ApplicationException {
        return userService.changePassword(getAuthenticatedUserName(), body);
    }

    @GetMapping("/confirm")
    public void confirmMail(@RequestParam("emailToken") String emailToken) throws ApplicationException {
        userService.confirm(emailToken);
    }

    @PostMapping("/resendConfirm")
    public void resendConfirm(@RequestBody @Valid UserConfirmIn userConfirmIn) throws ApplicationException {
        userService.sendConfirm(userConfirmIn);
    }

    @PreAuthorize("hasAnyAuthority('USER','MODERATOR','ADMIN')")
    @GetMapping("/{summonerName}")
    public UserPublicOut getUserBySummonerName(HttpServletRequest requestIncludesActualRole, @PathVariable String summonerName) throws ApplicationException {
        return userService.getUserPublicOut(summonerName);
    }
}

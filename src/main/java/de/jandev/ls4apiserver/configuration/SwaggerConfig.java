package de.jandev.ls4apiserver.configuration;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.bind.annotation.RequestMethod;
import springfox.documentation.builders.ApiInfoBuilder;
import springfox.documentation.builders.PathSelectors;
import springfox.documentation.builders.RequestHandlerSelectors;
import springfox.documentation.builders.ResponseMessageBuilder;
import springfox.documentation.service.ApiInfo;
import springfox.documentation.service.Contact;
import springfox.documentation.spi.DocumentationType;
import springfox.documentation.spring.web.plugins.Docket;
import springfox.documentation.swagger2.annotations.EnableSwagger2;

import java.util.Arrays;

@Configuration
@EnableSwagger2
public class SwaggerConfig {

    private static final String ERROR_MESSAGE_INTERNAL = "Internal server error.";
    private static final String ERROR_MESSAGE_UNAUTHORIZED = "JWT Token invalid or expired. / Password incorrect.";

    @Bean
    public Docket ls4Api() {
        return new Docket(DocumentationType.SWAGGER_2)
                .select()
                .apis(RequestHandlerSelectors.basePackage("de.jandev.ls4apiserver.web"))
                .paths(PathSelectors.regex("/.*"))
                .build()
                .apiInfo(metaData())
                .globalResponseMessage(RequestMethod.GET, Arrays.asList(new ResponseMessageBuilder().code(401).message(ERROR_MESSAGE_UNAUTHORIZED).build(), new ResponseMessageBuilder().code(500).message(ERROR_MESSAGE_INTERNAL).build()))
                .globalResponseMessage(RequestMethod.POST, Arrays.asList(new ResponseMessageBuilder().code(401).message(ERROR_MESSAGE_UNAUTHORIZED).build(), new ResponseMessageBuilder().code(500).message(ERROR_MESSAGE_INTERNAL).build()))
                .globalResponseMessage(RequestMethod.PUT, Arrays.asList(new ResponseMessageBuilder().code(401).message(ERROR_MESSAGE_UNAUTHORIZED).build(), new ResponseMessageBuilder().code(500).message(ERROR_MESSAGE_INTERNAL).build()))
                .globalResponseMessage(RequestMethod.DELETE, Arrays.asList(new ResponseMessageBuilder().code(401).message(ERROR_MESSAGE_UNAUTHORIZED).build(), new ResponseMessageBuilder().code(500).message(ERROR_MESSAGE_INTERNAL).build()));
    }

    private ApiInfo metaData() {
        return new ApiInfoBuilder()
                .title("LeagueS4 API Server REST API")
                .description("LeagueS4 API Server REST API documentation for internal usage.")
                .version("1.0.0")
                .license("GNU Affero General Public License v3.0")
                .licenseUrl("https://choosealicense.com/licenses/agpl-3.0/")
                .contact(new Contact("Jan Schipper", "https://leagues4.com/", "business@jandev.de"))
                .build();
    }
}

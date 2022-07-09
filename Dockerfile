FROM adoptopenjdk:11-jdk-hotspot

COPY target/ls4-api-server-0.0.1-SNAPSHOT.jar /backend.jar

RUN apt update -y && apt install git wget -y && wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb && dpkg -i packages-microsoft-prod.deb && apt update -y && apt install apt-transport-https dotnet-sdk-3.1 -y && git clone --single-branch --branch JanServer-new https://github.com/cabeca1143/GameServer.git && cd GameServer && git submodule init && git submodule update && dotnet build .

EXPOSE 80

ENTRYPOINT ["java", \
  "-Djava.security.egd=file:/dev/./urandom", \
  "-jar", "/backend.jar", \
  "--spring.profiles.active=production"]
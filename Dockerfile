# Base on dotnet 7
FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

# Install Base Dependencies
RUN apt-get update && apt-get install -y \
  curl \
  unzip \
  xvfb \
  libxi6 \
  libgconf-2-4 \
  wget

# Install Chrome
RUN wget https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb
RUN apt install -y ./google-chrome-stable_current_amd64.deb

# Install ChromeDriver
RUN wget https://chromedriver.storage.googleapis.com/86.0.4240.22/chromedriver_linux64.zip
RUN unzip chromedriver_linux64.zip
RUN mv chromedriver /usr/bin/chromedriver
RUN chown root:root /usr/bin/chromedriver
RUN chmod +x /usr/bin/chromedriver

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY USTACLI/usta-cli.csproj ./
RUN dotnet restore "usta-cli.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "USTACLI/usta-cli.csproj" -c Release -o /app/build

# Build and publish a release
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish -p:PublishChromeDriver=true

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "usta-cli.dll"]
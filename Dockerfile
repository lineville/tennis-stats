# Base on dotnet 7
FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY usta-scraper.csproj ./
RUN dotnet restore "usta-scraper.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "usta-scraper.csproj" -c Release -o /app/build


# Install Chrome and other dependencies
RUN apt update && apt install -y  \
  apt-transport-https \
  ca-certificates \
  curl \
  gnupg \
  hicolor-icon-theme \
  libcanberra-gtk* \
  libgl1-mesa-dri \
  libgl1-mesa-glx \
  libpango1.0-0 \
  libpulse0 \
  libv4l-0 \
  fonts-symbola \
  --no-install-recommends \
  && curl -sSL https://dl.google.com/linux/linux_signing_key.pub | apt-key add - \
  && echo "deb [arch=amd64] https://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google.list \
  && apt-get update && apt-get install -y \
  google-chrome-stable \
  --no-install-recommends \
  && apt-get purge --auto-remove -y curl \
  && rm -rf /var/lib/apt/lists/*

# Build and publish a release
FROM build AS publish
RUN dotnet publish -p:PublishChromeDriver=true -c Release -o /app/publish -r linux-x64 /p:PublishTrimmed=true /p:UseAppHost=false

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "usta-scraper.dll"]
# Base on dotnet 7
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

ADD https://dl.google.com/linux/direct/google-talkplugin_current_amd64.deb /src/google-talkplugin_current_amd64.deb

# Install Chrome
RUN apt-get update && apt-get install -y \
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

# Download the google-talkplugin And ChromeDrive
RUN set -x \
  && apt-get update \
  && apt-get install -y --no-install-recommends \
  ca-certificates \
  curl \
  unzip \
  && rm -rf /var/lib/apt/lists/* \
  && curl -sSL "https://dl.google.com/linux/direct/google-talkplugin_current_amd64.deb" -o /tmp/google-talkplugin-amd64.deb \
  && dpkg -i /tmp/google-talkplugin-amd64.deb \
  && mkdir \opt\selenium \
  && curl -sSL "https://chromedriver.storage.googleapis.com/2.40/chromedriver_linux64.zip" -o /tmp/chromedriver.zip \
  && unzip -o /tmp/chromedriver -d /opt/selenium/ \
  && rm -rf /tmp/*.deb \
  && apt-get purge -y --auto-remove curl unzip

# Add chrome user
RUN groupadd -r chrome && useradd -r -g chrome -G audio,video chrome \
  && mkdir -p /home/chrome/Downloads && chown -R chrome:chrome /home/chrome 

# Copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore

# Build and publish a release
COPY . .
RUN dotnet publish -p:PublishChromeDriver=true -c Release -o /app 

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:7.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "usta-scraper.dll"]
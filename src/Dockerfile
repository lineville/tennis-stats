# Base on dotnet 7
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Install Chrome WebDriver
RUN CHROMEDRIVER_VERSION=`curl -sS chromedriver.storage.googleapis.com/LATEST_RELEASE` && \
  mkdir -p /opt/chromedriver-$CHROMEDRIVER_VERSION && \
  curl -sS -o /tmp/chromedriver_linux64.zip http://chromedriver.storage.googleapis.com/$CHROMEDRIVER_VERSION/chromedriver_linux64.zip && \
  unzip -qq /tmp/chromedriver_linux64.zip -d /opt/chromedriver-$CHROMEDRIVER_VERSION && \
  rm /tmp/chromedriver_linux64.zip && \
  chmod +x /opt/chromedriver-$CHROMEDRIVER_VERSION/chromedriver && \
  ln -fs /opt/chromedriver-$CHROMEDRIVER_VERSION/chromedriver /usr/local/bin/chromedriver

# Install Google Chrome
RUN curl -sS -o - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - && \
  echo "deb http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google-chrome.list && \
  apt-get -yqq update && \
  apt-get -yqq install google-chrome-stable && \
  rm -rf /var/lib/apt/lists/*

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
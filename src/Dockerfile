# Base on dotnet 7
FROM ubuntu:latest AS build
WORKDIR /src

# Get .NET 7 SDK
RUN sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-7.0

# Copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore

# Install chrome
RUN wget https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb && \
  sudo dpkg -i google-chrome-stable_current_amd64.deb && \  
  sudo apt-get install -f

# Install chromedriver
RUN wget https://chromedriver.storage.googleapis.com/92.0.4515.107/chromedriver_linux64.zip && \
  unzip chromedriver_linux64.zip && \
  sudo mv chromedriver /usr/bin/chromedriver && \
  sudo chown root:root /usr/bin/chromedriver && \
  sudo chmod +x /usr/bin/chromedriver

# Build and publish a release
COPY . .
RUN dotnet publish -p:PublishChromeDriver=true -c Release -o /app 

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:7.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "usta-scraper.dll"]
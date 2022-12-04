# Base on dotnet 7
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore

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
COPY . ./
RUN dotnet publish -p:PublishChromeDriver=true -c Release -o out --self-contained -r linux-x64 /p:PublishTrimmed=true

# Build runtime image
FROM mcr.microsoft.com/dotnet/sdk:7.0 as runtime
WORKDIR /app

COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "usta-scraper.dll"]
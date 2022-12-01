# Base on dotnet 7
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore

# Build and publish a release
COPY . ./
RUN dotnet publish -p:PublishChromeDriver=true -c Release -o out --self-contained -r linux-x64 /p:PublishTrimmed=true

# Build runtime image
FROM selenium/standalone-chrome
WORKDIR /src

COPY --from=build /src/out .
ENTRYPOINT ["./usta-scraper"]
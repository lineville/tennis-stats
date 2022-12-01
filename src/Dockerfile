# Base on dotnet 7
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore

# Build and publish a release
COPY . .
RUN dotnet publish usta-scraper.csproj -p:PublishChromeDriver=true -c Release -o /app --self-contained -r linux-x64

# Build runtime image
FROM selenium/standalone-chrome
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./usta-scraper"]
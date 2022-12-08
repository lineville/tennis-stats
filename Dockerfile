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

# Build and publish a release
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish --self-contained -r linux-x64 /p:PublishChromeDriver=true /p:PublishTrimmed=true

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "usta-scraper.dll"]
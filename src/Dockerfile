# Base on dotnet 7
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

RUN apt-get update && \
  apt-get install -y google-chrome-stable libnss3 libgconf-2-4

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
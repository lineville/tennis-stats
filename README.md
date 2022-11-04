# USTA Ranking Web Scraper

Utility to scrape the USTA rankings page to get a player's current ranking

## Usage

Configure the following environment variables in `appsettings.json`

```json
  "Query": {
    "searchText": "Liam Neville",
    "ntrp-matchFormat": "SINGLES",
    "ntrp-rankListGender": "M",
    "ntrp-ntrpPlayerLevel": "level_4_0",
    "ntrp-sectionCode": "S50",
    "email": "email@emailcom"
  }
```

Run the application

```console
dotnet run
```

## Actions

- [ ] Scrape the page every 2 weeks, and send an email (maybe only if rank is diff than last time)

# USTA Ranking Web Scraper

Utility to scrape the USTA rankings page to get a player's current ranking

## Run with Docker

- Install Docker (if not already installed)

```console
docker build -t usta-scraper .
docker run --rm usta-scraper --name 'Liam Neville' \
                             --format SINGLES \
                             --gender M \
                             --level level_4_0 \
                             --section 'Northern California'
```

### CLI Options

- `--help` or `-h` - Display the help screen
- `--version` or `-v` - Display version information
- `--name` or `-n` - The name of the player to search for
- `--format` or `-f` - The format of the ranking to search for (SINGLES or DOUBLES)
- `--gender` or `-g` - The gender of the player to search for (M or F)
- `--level` or `-l` - The NTRP level of the player to search for, options include

  - level_3_0
  - level_3_5
  - level_4_0
  - level_4_5
  - level_5_0
  - level_5_5
  - level_6_0
  - level_6_5
  - level_7_0

- `--section` or `-s` - The section of the player to search for, options include

  - Eastern
  - Florida
  - Hawaii Pacific
  - Intermountain
  - Mid-Atlantic
  - Middle States
  - Midwest
  - Missouri Valley
  - New England
  - Northern California
  - Northern
  - Pacific NW
  - Southern
  - Southern California
  - Southwest
  - Texas
  - Unassigned

- (Optional) `--json` or `-j` - Output the results as JSON (Defaults to markdown)

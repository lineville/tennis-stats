# USTA Ranking Web Scraper

Utility to scrape the USTA rankings page to get a player's current ranking

## Usage

- Clone this repo
- Install .NET 7 (if not already installed)
- Install Chrome (if not already installed)

```console
dotnet run -- --name 'Liam Neville' \
              --format SINGLES \
              --gender M \
              --level level_4_0 \
              --section 'Northern California'
```

### CLI Options

- `--help` or `-h` - Display the help screen
- `--version` or `-v` - Display version information
- `--name` or `-n` - The name of the player to search for
  - Replaced by environment variable `NTRP_NAME`
- `--format` or `-f` - The format of the ranking to search for (SINGLES or DOUBLES)
  - Replaced by environment variable `NTRP_FORMAT`
- `--gender` or `-g` - The gender of the player to search for (M or F)
  - Replaced by environment variable `NTRP_GENDER`
- `--level` or `-l` - The NTRP level of the player to search for, options include
  - Replaced by environment variable `NTRP_LEVEL`
  - Options include
    - level_3_0
    - level_3_5
    - level_4_0
    - level_4_5
    - level_5_0
    - level_5_5
    - level_6_0
    - level_6_5
    - level_7_0

- `--section` or `-s` - The section of the player to search for, o
  - Replaced by environment variable `NTRP_SECTION`
  - Options include
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

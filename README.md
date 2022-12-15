# USTA Ranking Web Scraper

Tool to scrape the USTA rankings page to get a player's current ranking and send scheduled email updates when new rankings are posted

## Prerequisites

- Install [Docker](https://docs.docker.com/get-docker/)
- Create a [GitHub PAT](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token) with `read:packages` scope

## Usage

```console
## Login to Docker
$ echo <YOUR_PAT> | docker login ghcr.io -u <YOUR_USERNAME> --password-stdin

## Pull the latest container
$ docker pull ghcr.io/lineville/usta-scraper:latest

## Search for a player's ranking
$ docker run ghcr.io/lineville/usta-scraper -n "Liam Neville" -f SINGLES -g M -l 4.0 -s "Northern California"

## Liam Neville

### Northern California Men's 4.0 singles

- National Rank: 352
- Section Rank: 18
- District Rank: 18
```

### CLI Options

- `--help` or `-h` - Display the help screen
- `--version` or `-v` - Display version information
- `--name` or `-n` - The name of the player to search for
- `--format` or `-f` - The format of the ranking to search for (SINGLES or DOUBLES)
- `--gender` or `-g` - The gender of the player to search for (M or F)
- `--level` or `-l` - The NTRP level of the player to search for, options include

  - 3.0
  - 3.5
  - 4.0
  - 4.5
  - 5.0
  - 5.5
  - 6.0
  - 6.5
  - 7.0

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

- (Optional) `--output` or `-o` - Defaults to markdown, options include
  - html
  - json

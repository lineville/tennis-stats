# USTA CLI

Command Line Tool to scrape the USTA rankings page to get a player's current ranking and send scheduled email updates when new rankings are posted

## Usage

- Install docker locally or open in GitHub CodeSpaces

```console
docker run -t ghcr.io/lineville/usta-cli -n "Liam Neville" -f SINGLES -g M -l 4.0 -s "Northern California"
```

### Example Output

```markdown
## Liam Neville

### Northern California Men's 4.0 singles

- National Rank: 349
- Section Rank: 16
- District Rank: 16
```

### CLI Options

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
- (Optional) `--help` or `-h` - Display the help screen
- (Optional) `--version` or `-v` - Display version information

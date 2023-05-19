# USTA CLI 🎾

## [![🚀 Release](https://github.com/lineville/usta-cli/actions/workflows/release.yml/badge.svg)](https://github.com/lineville/usta-cli/actions/workflows/release.yml) [![🧪 CI](https://github.com/lineville/usta-cli/actions/workflows/ci.yml/badge.svg)](https://github.com/lineville/usta-cli/actions/workflows/ci.yml) [![🏆 Update Rank](https://github.com/lineville/usta-cli/actions/workflows/update_rank.yml/badge.svg)](https://github.com/lineville/usta-cli/actions/workflows/update_rank.yml)

Command Line Tool to scrape the USTA rankings page to get a player's current ranking and send scheduled email updates when new rankings are posted

## Usage

* Install [docker](https://docs.docker.com/get-docker/) or open in a GitHub CodeSpace 🚀

```console
docker run -it ghcr.io/lineville/usta-cli <command> [options]
```

## Commands

### `rankings list [options]`

Lists the top 20 ranked players in a given section, format, and level

> All of the options listed below are optional (**not required**), if any of these are not provided it will give you an interactive prompt to select the options. If you wish to skip the interactive prompt or run this from an automated context, you must provide all of the options listed below.

* `--format` or `-f` - The match format
  * SINGLES
  * DOUBLES
* `--gender` or `-g` - The player's listed gender
  * M
  * F
* `--level` or `-l` - The NTRP level

  * 3.0
  * 3.5
  * 4.0
  * 4.5
  * 5.0

* `--section` or `-s` - The USTA section

  * Eastern
  * Florida
  * Hawaii Pacific
  * Intermountain
  * Mid-Atlantic
  * Middle States
  * Midwest
  * Missouri Valley
  * New England
  * Northern California
  * Northern
  * Pacific NW
  * Southern
  * Southern California
  * Southwest
  * Texas
  * Unassigned

#### Example `rankings list` Output

![0B48D7E0-4D69-46D6-AB35-E3456F46A793](https://user-images.githubusercontent.com/25349044/215961392-42bc161d-9342-4e8c-a727-72b86db979bf.jpeg)

---

### `rankings get [options]`

Gets the ranking of a single player in a given section, format, and level

> All of the options listed below are optional (**not required**), if any of these are not provided it will give you an interactive prompt to select the options. If you wish to skip the interactive prompt or run this from an automated context, you must provide all of the options listed below.

* `--name` or `-n` - The player's full name
* `--format` or `-f` - The match format
  * SINGLES
  * DOUBLES
* `--gender` or `-g` - The player's listed gender
  * M
  * F
* `--level` or `-l` - The NTRP level
  * 3.0
  * 3.5
  * 4.0
  * 4.5
  * 5.0
* `--section` or `-s` - The USTA section

  * Eastern
  * Florida
  * Hawaii Pacific
  * Intermountain
  * Mid-Atlantic
  * Middle States
  * Midwest
  * Missouri Valley
  * New England
  * Northern California
  * Northern
  * Pacific NW
  * Southern
  * Southern California
  * Southwest
  * Texas
  * Unassigned

* (Optional) `--output` or `-o` - Defaults to markdown, options include
  * html
  * json

#### Example `rankings get` Output

```markdown
## Liam Neville

### Northern California Men's 4.0 singles

- National Rank: 349
- Section Rank: 16
- District Rank: 16
```

### `rankings subscribe [options]`

Subscribes to a player's rankings updates and sends a weekly email when new rankings are posted

> All of the options listed below are optional (**not required**), if any of these are not provided it will give you an interactive prompt to select the options. If you wish to skip the interactive prompt or run this from an automated context, you must provide all of the options listed below.

* `--name` or `-n` - The player's full name
* `--format` or `-f` - The match format
  * SINGLES
  * DOUBLES
* `--gender` or `-g` - The player's listed gender
  * M
  * F
* `--level` or `-l` - The NTRP level
  * 3.0
  * 3.5
  * 4.0
  * 4.5
  * 5.0
* `--section` or `-s` - The USTA section

  * Eastern
  * Florida
  * Hawaii Pacific
  * Intermountain
  * Mid-Atlantic
  * Middle States
  * Midwest
  * Missouri Valley
  * New England
  * Northern California
  * Northern
  * Pacific NW
  * Southern
  * Southern California
  * Southwest
  * Texas
  * Unassigned

#### Example `rankings subscribe` Output

```console
Successfully subscribed to rankings updates for Liam Neville, level 4.0, section Northern California, format SINGLES
```

### `rankings unsubscribe [options]`

Unsubscribes from a player's rankings updates

> All of the options listed below are optional (**not required**), if any of these are not provided it will give you an interactive prompt to select the options. If you wish to skip the interactive prompt or run this from an automated context, you must provide all of the options listed below.

* `--name` or `-n` - The player's full name
* `--format` or `-f` - The match format
  * SINGLES
  * DOUBLES
* `--gender` or `-g` - The player's listed gender
  * M
  * F
* `--level` or `-l` - The NTRP level
  * 3.0
  * 3.5
  * 4.0
  * 4.5
  * 5.0
* `--section` or `-s` - The USTA section

  * Eastern
  * Florida
  * Hawaii Pacific
  * Intermountain
  * Mid-Atlantic
  * Middle States
  * Midwest
  * Missouri Valley
  * New England
  * Northern California
  * Northern
  * Pacific NW
  * Southern
  * Southern California
  * Southwest
  * Texas
  * Unassigned

#### Example `rankings unsubscribe` Output

```console
Successfully unsubscribed to rankings updates for Liam Neville, level 4.0, section Northern California, format SINGLES
```

# USTA CLI 🎾

## [![🚀 Release](https://github.com/lineville/usta-cli/actions/workflows/release.yml/badge.svg)](https://github.com/lineville/usta-cli/actions/workflows/release.yml) [![🧪 CI](https://github.com/lineville/usta-cli/actions/workflows/ci.yml/badge.svg)](https://github.com/lineville/usta-cli/actions/workflows/ci.yml) [![🏆 Check Ranking](https://github.com/lineville/usta-cli/actions/workflows/check_rank.yml/badge.svg)](https://github.com/lineville/usta-cli/actions/workflows/check_rank.yml)

Command Line Tool to scrape the USTA rankings page to get a player's current ranking and send scheduled email updates when new rankings are posted

## Requirements

Install [dotnet](https://dotnet.microsoft.com/en-us/download)

## Usage

```console
dotnet tool install --global usta-cli
usta-cli <command> [options]
```

## Commands

### `get [options]`

Gets a player's current ranking

- `-n | --name ['FIRST LAST']`
- `-f | --format [SINGLES|DOUBLES]`
- `-g | --gender [M|F]`
- `-l | --level [3.0|3.5|4.0|4.5|5.0]`
- `-s | --section [Eastern|Florida|Hawaii Pacific|Intermountain|Mid-Atlantic|Middle States|Midwest|Missouri Valley|New England|Northern California|Northern|Pacific NW|Southern|Southern California|Southwest|Texas|Unassigned]`
- `-o | --output [html|json]`

---

### `list [options]`

Lists the top 20 players in a given section and level

- `-f | --format [SINGLES|DOUBLES]`
- `-g | --gender [M|F]`
- `-l | --level [3.0|3.5|4.0|4.5|5.0]`
- `-s | --section [Eastern|Florida|Hawaii Pacific|Intermountain|Mid-Atlantic|Middle States|Midwest|Missouri Valley|New England|Northern California|Northern|Pacific NW|Southern|Southern California|Southwest|Texas|Unassigned]`
- `-o | --output [html|json]`

---

### `subscribe [options]`

Subscribes to weekly email updates for a player's rankings

- `-n | --name ['FIRST LAST']`
- `-f | --format [SINGLES|DOUBLES]`
- `-g | --gender [M|F]`
- `-l | --level [3.0|3.5|4.0|4.5|5.0]`
- `-s | --section [Eastern|Florida|Hawaii Pacific|Intermountain|Mid-Atlantic|Middle States|Midwest|Missouri Valley|New England|Northern California|Northern|Pacific NW|Southern|Southern California|Southwest|Texas|Unassigned]`
- `-o | --output [html|json]`

---

### `unsubscribe [options]`

Unsubscribes to weekly email updates for a player's rankings

- `-n | --name ['FIRST LAST']`
- `-f | --format [SINGLES|DOUBLES]`
- `-g | --gender [M|F]`
- `-l | --level [3.0|3.5|4.0|4.5|5.0]`
- `-s | --section [Eastern|Florida|Hawaii Pacific|Intermountain|Mid-Atlantic|Middle States|Midwest|Missouri Valley|New England|Northern California|Northern|Pacific NW|Southern|Southern California|Southwest|Texas|Unassigned]`
- `-o | --output [html|json]`

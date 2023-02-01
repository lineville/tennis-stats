using System.ComponentModel;
using Spectre.Console.Cli;

public class RankingsSettings : CommandSettings
{
  [CommandOption("-n|--name")]
  public string? Name { get; set; }

  [CommandOption("-f|--format")]
  public MatchFormat? Format { get; set; }

  [CommandOption("-g|--gender")]
  public Gender? Gender { get; set; }

  [CommandOption("-l|--level")]
  public string? Level { get; set; }

  [CommandOption("-s|--section")]
  public string? Section { get; set; }

  [CommandOption("-o|--output")]
  public Output? Output { get; set; }
}

public enum MatchFormat
{
  SINGLES,
  DOUBLES
}

public enum Gender
{
  M,
  F
}

public enum Output
{
  json,
  html
}
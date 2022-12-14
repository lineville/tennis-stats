using CommandLine;
public class CLIOptions
{
  [Option('n', "name", Required = true, HelpText = "Player name")]
  public string? Name { get; set; }

  [Option('f', "format", Required = true, HelpText = "Match format ('SINGLES' or 'DOUBLES')")]
  public string? Format { get; set; }

  [Option('g', "gender", Required = true, HelpText = "Gender ('M' or 'F')")]
  public string? Gender { get; set; }

  [Option('l', "level", Required = false, HelpText = "NTRP level ('level_3_0', 'level_3_5', 'level_4_0', 'level_4_5', 'level_5_0', 'level_5_5', 'level_6_0', 'level_6_5', 'level_7_0')")]
  public string? Level { get; set; }

  [Option('s', "section", Required = true, HelpText = "NTRP Section Code (Options can be found at https://github.com/lineville/usta-scraper)")]
  public string? Section { get; set; }

  [Option('o', "output", Required = false, HelpText = "Output format ('json' or 'html')")]
  public string? Output { get; set; }
}

using CommandLine;
public class CLIOptions
{
    [Option('n', "name", Required = true, HelpText = "Player name")]
    public string? Name { get; set; }

    [Option('f', "format", Required = true, HelpText = "Match format ('SINGLES' or 'DOUBLES')")]
    public MatchFormat Format { get; set; }

    [Option('g', "gender", Required = true, HelpText = "Gender ('M' or 'F')")]
    public Gender Gender { get; set; }

    [Option('l', "level", Required = false, HelpText = "NTRP level ('3.0', '3.5', '4.0', '4.5', '5.0', '5.5', '6.0', '6.5', '7.0')")]
    public string? Level { get; set; }

    [Option('s', "section", Required = true, HelpText = "NTRP Section Code (Options can be found at https://github.com/lineville/usta-cli)")]
    public string? Section { get; set; }

    [Option('o', "output", Required = false, HelpText = "Output format ('json' or 'html')")]
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
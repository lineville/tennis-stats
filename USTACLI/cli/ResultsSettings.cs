using Spectre.Console.Cli;

public class ResultsSettings : CommandSettings
{
    [CommandArgument(0, "[NAME]")]
    public string? Name { get; set; }
}
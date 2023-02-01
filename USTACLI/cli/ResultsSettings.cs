using Spectre.Console.Cli;

public class ResultsSettings : CommandSettings
{
    [CommandArgument(0, "[NAME]")]
    public string? Name { get; set; }

    [CommandOption("-u|--usta-number <USTA_NUMBER>")]
    public string? USTANumber { get; set; }
}
using Spectre.Console.Cli;

public class ResultsCommand : Command<ResultsSettings>
{
  public override int Execute(CommandContext context, ResultsSettings settings)
  {
    // Either need to pass in a USTA number directly (optional) or a name
    // If name is passed in, search for the player by name at https://www.usta.com/en/home/play/player-search.html

    // Then scrape the data from that page.

    // If USTA number is passed in, scrape the data from https://www.usta.com/en/home/play/player-search/profile.html#?uaid=<USTA_NUMBER>%23tab=results
    return 0;
  }
}
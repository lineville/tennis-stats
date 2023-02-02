using Spectre.Console.Cli;

public class UnsubscribeRankingsCommand : Command<RankingsSettings>
{
#nullable disable
  public override int Execute(CommandContext context, RankingsSettings settings)
  {
#nullable enable

    // Invoke Javascript
    // Call pocketbase db to remove the user from the collection
    return 0;
  }
}
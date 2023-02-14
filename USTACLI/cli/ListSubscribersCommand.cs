using Spectre.Console.Cli;
using StackExchange.Redis;

public class ListSubscribersCommand : Command
{
#nullable disable
  public override int Execute(CommandContext context)
  {
#nullable enable

    // TODO read from redis
    Console.WriteLine("liamgneville@gmail.com");
    Console.WriteLine("lineville@github.com");
    return 0;
  }
}
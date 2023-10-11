using Spectre.Console.Cli;

public class Program
{
  /// <summary>
  /// Main entry point
  /// </summary>
  public static int Main(string[] args)
  {
    // Parse command line arguments
    var app = new CommandApp();
    app.Configure(config =>
    {
      config.AddCommand<GetRankingsCommand>("get");
      config.AddCommand<ListRankingsCommand>("list");
      config.AddCommand<SubscribeRankingsCommand>("subscribe");
      config.AddCommand<UnsubscribeRankingsCommand>("unsubscribe");

      config.AddBranch("subscribers", subscribers =>
      {
        subscribers.AddCommand<ListSubscribersCommand>("list");
      });
    });

    return app.Run(args);
  }
}
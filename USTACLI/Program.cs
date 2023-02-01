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
            config.AddBranch<RankingsSettings>("rankings", rankings =>
            {
                rankings.AddCommand<GetRankingsCommand>("get");
                rankings.AddCommand<ListRankingsCommand>("list");
            });
            // config.AddCommand<ScheduleCommand>("results");
        });

        return app.Run(args);
    }
}
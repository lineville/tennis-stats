using Spectre.Console.Cli;
using Spectre.Console;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;

public class ListRankingsCommand : Command<RankingsSettings>
{
#nullable disable
  public override int Execute(CommandContext context, RankingsSettings settings)
  {
#nullable enable

    // Load appsettings.json static data
    IConfiguration configuration = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: false)
      .Build();

    Utilities.InteractiveFallback(settings, configuration, context.Name);

    List<Player> players = new List<Player>();

    AnsiConsole
    .Status()
    .Spinner(new TennisBallSpinner())
    .Start("Searching for rankings", _ctx =>
    {
      // Create the chrome driver service
      using (var driver = Driver.Create())
      {
        // Scrape the player ranking
        players.AddRange(ScrapeRankings(driver, configuration, settings, context.Name));
      }
    });

    var table = new Table();
    table.Title = new TableTitle($"{settings.Section} {(settings.Gender == Gender.M ? "Men's" : "Women's")} {settings.Level} {settings.Format} USTA Rankings", new Style(Color.Aqua, Color.Black));
    table.Border = TableBorder.HeavyEdge;

    // Add header row 
    table.AddColumns(new TableColumn[]
      {
        new TableColumn(new Text("Name", new Style(Color.Blue, Color.Black)).LeftJustified()),
        new TableColumn(new Text("District", new Style(Color.Red, Color.Black)).LeftJustified()),
        new TableColumn(new Text("Section", new Style(Color.Yellow, Color.Black)).LeftJustified()),
        new TableColumn(new Text("National", new Style(Color.Green, Color.Black)).LeftJustified()),
        new TableColumn(new Text("Points", new Style(Color.Gold1, Color.Black)).LeftJustified()),
        new TableColumn(new Text("Location", new Style(Color.Purple, Color.Black)).LeftJustified())
      });

    foreach (var player in players)
    {
      table.AddRow(new string[] { player.Name.ToString(), player.DistrictRank.ToString(), player.SectionRank.ToString(), player.NationalRank.ToString(), player.TotalPoints.ToString(), player.Location.ToString() });
    }

    // Write to Console
    AnsiConsole.Clear();
    AnsiConsole.Write(table);
    AnsiConsole.WriteLine();

    return 0;
  }

  /// <summary>
  /// Extracts the HTML element and returns a Player object
  /// </summary>
  public static List<Player> ScrapeRankings(WebDriver driver, IConfiguration configuration, RankingsSettings settings, string context)
  {
    var htmlElement = configuration.GetValue<string>("HTML_ELEMENT_TARGET")
      ?? throw new Exception("Failed to load HTML_ELEMENT_TARGET from appsettings.json");

    var timeout = configuration.GetValue<int>("PAGE_LOAD_TIMEOUT");

    var url = Utilities.BuildUSTARankingURL(settings, configuration, context);

    // Navigate to the URL and wait for the page to load
    driver.Navigate().GoToUrl(url);
    Thread.Sleep(timeout * 1000);

    var elements = driver.FindElements(By.ClassName(htmlElement));

    var playerElements = elements.Chunk(9);

    return playerElements.Select(p => new Player(p[3].Text, p[0].Text, p[1].Text, p[2].Text, p[4].Text, p[5].Text)).ToList();
  }
}
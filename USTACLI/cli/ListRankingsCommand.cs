using Spectre.Console.Cli;
using Spectre.Console;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.WebUtilities;
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

    InteractiveFallback(settings, configuration);

    // Construct the URL from cli args
    var url = BuildUSTARankingURL(settings, configuration);


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
        players.AddRange(ScrapeRankings(driver, url, configuration, settings));
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
  public static List<Player> ScrapeRankings(WebDriver driver, string url, IConfiguration configuration, RankingsSettings settings)
  {
    var htmlElement = configuration.GetValue<string>("HTML_ELEMENT_TARGET")
      ?? throw new Exception("Failed to load HTML_ELEMENT_TARGET from appsettings.json");

    var timeout = configuration.GetValue<int>("PAGE_LOAD_TIMEOUT");

    // Navigate to the URL and wait for the page to load
    driver.Navigate().GoToUrl(url);
    Thread.Sleep(timeout * 1000);

    var elements = driver.FindElements(By.ClassName(htmlElement));

    var playerElements = elements.Chunk(9);

    return playerElements.Select(p => new Player(p[3].Text, p[0].Text, p[1].Text, p[2].Text, p[4].Text, p[5].Text)).ToList();
  }


  /// <summary>
  /// Interactive prompts to fill in missing CLI settings
  /// <summary>
  public static void InteractiveFallback(RankingsSettings settings, IConfiguration configuration)
  {

    if (settings.Level == null)
    {
      var level = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .Title("Select [aqua]NTRP level[/]")
        .PageSize(10)
        .AddChoices(new[] { "3.0", "3.5", "4.0", "4.5", "5.0" }));
      settings.Level = level;
    }

    if (settings.Gender == null)
    {
      var gender = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .Title("Select [aqua]gender[/]")
        .PageSize(10)
        .AddChoices(new[] { "M 👨", "F 👩" }));
      settings.Gender = gender.StartsWith("M") ? Gender.M : Gender.F;
    }

    if (settings.Format == null)
    {
      var format = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .Title("Select [aqua]match format[/]")
        .PageSize(10)
        .AddChoices(new[]
          { $"SINGLES {(settings.Gender == Gender.M ? "🙋" : "🙋‍♀️")}",
            $"DOUBLES {(settings.Gender == Gender.M ? "👬" : "👭")}"
          }
        )
      );
      settings.Format = format.StartsWith("SINGLES") ? MatchFormat.SINGLES : MatchFormat.DOUBLES;
    }

    if (settings.Section == null)
    {
      var sectionNames = configuration.GetRequiredSection("SECTION_CODES").Get<Dictionary<string, string>>()?.Keys ?? throw new Exception("Failed to load SECTION_CODES from appsettings.json");
      var section = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .Title("Select [aqua]USTA section[/]")
        .PageSize(20)
        .AddChoices(sectionNames));
      settings.Section = section;
    }
  }

  /// <summary>
  /// Construct the correct URL from CLI args
  /// </summary>
  public static string BuildUSTARankingURL(RankingsSettings settings, IConfiguration configuration)
  {
    var queryKeys = configuration.GetRequiredSection("QUERY_PARAMS").Get<Dictionary<string, string>>() ?? throw new Exception("Failed to load QUERY_PARAMS from appsettings.json");
    var sectionCodes = configuration.GetRequiredSection("SECTION_CODES").Get<Dictionary<string, string>>() ?? throw new Exception("Failed to load SECTION_CODES from appsettings.json");
    var ustaBaseURL = configuration.GetValue<string>("USTA_BASE_URL") ?? throw new Exception("Failed to load USTA_BASE_URL from appsettings.json");

    // Build the query string
    var queryParams = new Dictionary<string, string>()
    {
      { queryKeys["Format"], settings.Format?.ToString() ?? ""},
      { queryKeys["Gender"], settings.Gender?.ToString() ?? ""},
      { queryKeys["Level"], "level_" + settings.Level?.ToString().Replace(".", "_")},
      { queryKeys["Section"], sectionCodes[settings.Section ?? ""] }
    };

    var url = QueryHelpers.AddQueryString(ustaBaseURL, queryParams);

    // Workaround for weird # getting ignored in QueryHelpers
    url = url.Insert(ustaBaseURL.Count(), "#") + "#tab=ntrp";
    return url;
  }
}
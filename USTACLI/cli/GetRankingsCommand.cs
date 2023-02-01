using Spectre.Console.Cli;
using Spectre.Console;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

public class GetRankingsCommand : Command<RankingsSettings>
{
  public override int Execute(CommandContext context, RankingsSettings settings)
  {
    // Load appsettings.json static data
    IConfiguration configuration = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: false)
      .Build();

    InteractiveFallback(settings, configuration);

    // Construct the URL from cli args
    var url = BuildUSTARankingURL(settings, configuration);

    var maxRetries = configuration.GetValue<int>("MAX_RETRIES");
    var retries = 0;

    RankingsReport? player = null;

    AnsiConsole
    .Status()
    .Spinner(new TennisBallSpinner())
    .Start("Searching for rankings", _ctx =>
    {
      // Try a few times in case of failures
      while (retries < maxRetries && player == null)
      {
        // Create the chrome driver service
        using (var driver = Driver.Create())
        {
          try
          {
            // Scrape the player ranking
            player = ScrapePlayerRanking(driver, url, configuration, settings);
          }
          catch (Exception)
          {
            retries++;
          }
          finally
          {
            driver.Quit();
          }
        }
      }
    });

    if (player == null)
    {
      throw new Exception($"Failed to find ranking for {settings.Name}");
    }
    else
    {
      AnsiConsole.Clear();
      player.Print();
    }
    return 0;
  }

  /// <summary>
  /// Extracts the HTML element and returns a Player object
  /// </summary>
  public static RankingsReport ScrapePlayerRanking(WebDriver driver, string url, IConfiguration configuration, RankingsSettings settings)
  {
    var htmlElement = configuration.GetValue<string>("HTML_ELEMENT_TARGET")
      ?? throw new Exception("Failed to load HTML_ELEMENT_TARGET from appsettings.json");

    var timeout = configuration.GetValue<int>("PAGE_LOAD_TIMEOUT");

    // Navigate to the URL and wait for the page to load
    driver.Navigate().GoToUrl(url);
    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
    wait.Until(d => d.FindElement(By.ClassName(htmlElement)));

    var elements = driver.FindElements(By.ClassName(htmlElement));

    if (elements[3].Text != settings.Name) // Throw if the name doesn't match
    {
      throw new NotFoundException($"No ranking found for {settings.Name}");
    }

    return new RankingsReport()
    {
      Player = new Player(elements[3].Text, elements[0].Text, elements[1].Text, elements[2].Text, elements[3].Text, elements[4].Text),
      Settings = settings
    };
  }


  /// <summary>
  /// Interactive prompts to fill in missing CLI settings
  /// <summary>
  public static void InteractiveFallback(RankingsSettings settings, IConfiguration configuration)
  {
    if (settings.Name == null)
    {
      var name = AnsiConsole.Prompt(
        new TextPrompt<string>("What's your [aqua]name[/]?"));
      settings.Name = name;
    }

    if (settings.Level == null)
    {
      var level = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .Title("Select your [aqua]NTRP level[/]")
        .PageSize(10)
        .AddChoices(new[] { "3.0", "3.5", "4.0", "4.5", "5.0" }));
      settings.Level = level;
    }

    if (settings.Gender == null)
    {
      var gender = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .Title("Select your [aqua]gender[/]")
        .PageSize(10)
        .AddChoices(new[] { "M 👨", "F 👩" }));
      settings.Gender = gender.StartsWith("M") ? Gender.M : Gender.F;
    }

    if (settings.Format == null)
    {
      var format = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .Title("Select your [aqua]match format[/]")
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
        .Title("Select your [aqua]USTA section[/]")
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
      { queryKeys["Name"], settings.Name ?? "" },
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
using Spectre.Console.Cli;
using Spectre.Console;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium.Chrome;
using Microsoft.AspNetCore.WebUtilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

public class ListRankingsCommand : Command<RankingsSettings>
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


        List<Player> players = new List<Player>();

        AnsiConsole
        .Status()
        .Spinner(new TennisBallSpinner())
        .Start("Searching for rankings", _ctx =>
        {
            // Create the chrome driver service
            using (var driver = CreateChromeDriverService())
            {
                // Scrape the player ranking
                players.AddRange(ScrapeRankings(driver, url, configuration, settings));
            }
        });

        var grid = new Grid();

        // Add columns 
        grid.AddColumns(6);

        // Add header row 
        grid.AddRow(new Text[]
          {
        new Text("Name", new Style(Color.Blue, Color.Black)).LeftJustified(),
        new Text("District", new Style(Color.Red, Color.Black)).LeftJustified(),
        new Text("Section", new Style(Color.Yellow, Color.Black)).LeftJustified(),
        new Text("National", new Style(Color.Green, Color.Black)).LeftJustified(),
        new Text("Points", new Style(Color.Gold1, Color.Black)).LeftJustified(),
        new Text("Location", new Style(Color.Purple, Color.Black)).LeftJustified()
          });

        foreach (var player in players)
        {
            grid.AddRow(new string[] { player.Name.ToString(), player.DistrictRank.ToString(), player.SectionRank.ToString(), player.NationalRank.ToString(), player.TotalPoints.ToString(), player.Location.ToString() });
        }

        // Write to Console
        AnsiConsole.Clear();
        AnsiConsole.Write(grid);
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

        // driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeout);

        // Navigate to the URL and wait for the page to load
        driver.Navigate().GoToUrl(url);
        Thread.Sleep(timeout * 1000);
        // var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
        // wait.Until(d => d.FindElement(By.ClassName(htmlElement)));

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


    // TODO Make this a global singleton service that can be injected to each command
    /// <summary>
    /// Setup silent headless chrome driver service
    /// </summary>
    public static ChromeDriver CreateChromeDriverService()
    {
        ChromeDriverService service = ChromeDriverService.CreateDefaultService();
        service.SuppressInitialDiagnosticInformation = true;

        ChromeOptions options = new ChromeOptions() { PageLoadStrategy = PageLoadStrategy.Eager };

        options.AddArguments(
          "no-sandbox",
          "headless",
          "disable-gpu",
          "disable-logging",
          "disable-dev-shm-usage",
          "window-size=1920,1080",
          "disable-extensions",
          "log-level=OFF",
          "--user-agent=Chrome/73.0.3683.86",
          "output=/dev/null"
        );

        var driver = new ChromeDriver(service, options);
        return driver;
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
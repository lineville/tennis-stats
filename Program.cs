using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using CommandLine;

public class Program
{
  /// <summary>
  /// Construct the correct URL from CLI args
  /// </summary>
  private static string BuildUSTARankingURL(CLIOptions options, IConfiguration configuration)
  {
    var queryKeys = configuration.GetRequiredSection("QUERY_PARAMS").Get<Dictionary<string, string>>() ?? throw new Exception("Failed to load QUERY_PARAMS from appsettings.json");
    var sectionCodes = configuration.GetRequiredSection("SECTION_CODES").Get<Dictionary<string, string>>() ?? throw new Exception("Failed to load SECTION_CODES from appsettings.json");
    var ustaBaseURL = configuration.GetValue<string>("USTA_BASE_URL") ?? throw new Exception("Failed to load USTA_BASE_URL from appsettings.json");

    // Build the query string
    var queryParams = new Dictionary<string, string>()
    {
      { queryKeys["Name"], options.Name ?? "" },
      { queryKeys["Format"], options.Format ?? ""},
      { queryKeys["Gender"], options.Gender ?? "" },
      { queryKeys["Level"], options.Level ?? "" },
      { queryKeys["Section"], sectionCodes[options.Section ?? ""] }
    };

    var url = QueryHelpers.AddQueryString(ustaBaseURL, queryParams);

    // Workaround for weird # getting ignored in QueryHelpers
    url = url.Insert(ustaBaseURL.Count(), "#") + "#tab=ntrp";
    return url;
  }

  /// <summary>
  /// Setup silent headless chrome driver service
  /// </summary>
  private static ChromeDriver CreateChromeDriverService()
  {
    ChromeDriverService service = ChromeDriverService.CreateDefaultService();
    service.SuppressInitialDiagnosticInformation = true;

    ChromeOptions options = new ChromeOptions();
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
  /// Extracts the HTML element and returns a Player object
  /// </summary>
  private static Player ScrapePlayerRanking(WebDriver driver, string url, string name, IConfiguration configuration)
  {
    var htmlElement = configuration.GetValue<string>("HTML_ELEMENT_TARGET")
      ?? throw new Exception("Failed to load HTML_ELEMENT_TARGET from appsettings.json");

    var timeout = configuration.GetValue<int>("PAGE_LOAD_TIMEOUT");

    // Navigate to the URL and wait for the page to load
    driver.Navigate().GoToUrl(url);
    var elements = new WebDriverWait(driver, new TimeSpan(0, 0, timeout))
      .Until(d => d.FindElement(By.ClassName(htmlElement)))
      .FindElements(By.ClassName(htmlElement));

    if (elements[3].Text != name) // Throw if the name doesn't match
    {
      throw new NotFoundException($"No ranking found for {name}");
    }

    return new Player()
    {
      Name = elements[3].Text,
      NationalRank = int.Parse(elements[0].Text),
      SectionRank = int.Parse(elements[1].Text),
      DistrictRank = int.Parse(elements[2].Text)
    };
  }

  /// <summary>
  /// Main entry point
  /// </summary>
  private static void Main(string[] args)
  {
    // Load appsettings.json static data
    IConfiguration configuration = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: false)
      .Build();

    // Parse command line arguments
    var options = Parser.Default.ParseArguments<CLIOptions>(args).Value
      ?? throw new Exception("Failed to parse command line arguments");

    // Construct the URL from cli args
    var url = BuildUSTARankingURL(options, configuration);

    // Create the chrome driver service
    var driver = CreateChromeDriverService();

    var maxRetries = configuration.GetValue<int>("MAX_RETRIES");
    var retries = 0;
    var foundRanking = false;

    while (retries < maxRetries && foundRanking == false)
    {
      try
      {
        // Scrape the player ranking
        var player = ScrapePlayerRanking(driver, url, options.Name ?? "", configuration);

        // Print out the player ranking as JSON or markdown
        if (options.JSON == true)
        {
          Console.WriteLine(player.ToJSON());
        }
        else
        {
          Console.WriteLine(player.ToMarkDown(options));
        }

        foundRanking = true;
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

    if (foundRanking == false)
    {
      throw new Exception($"Failed to find ranking for {options.Name}");
    }
  }
}

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Microsoft.AspNetCore.WebUtilities;
using CommandLine;

public class Program
{
  // Specific CSS class used by the USTA website, this may change in the future...
  private static string HTML_ELEMENT_TARGET = "v-grid-cell__content";
  // USTA rankings base URL also subject to change in the future...
  private static string USTA_BASE_URL = "https://www.usta.com/en/home/play/rankings.html";
  // How long we wait for the page to load before we give up
  private static int PAGE_LOAD_TIMEOUT = 10;


  // Mapping NTRP sections to the section codes, scraped from the site internals...
  private static Dictionary<string, string> SECTION_CODES = new Dictionary<string, string>()
  {
    { "Eastern", "S10" },
    { "Florida", "S15" },
    { "Hawaii Pacific", "S20" },
    { "Intermountain", "S25" },
    { "Mid-Atlantic", "S30" },
    { "Middle States", "S35" },
    { "Midwest", "S85" },
    { "Missouri Valley", "S40" },
    { "New England", "S45" },
    { "Northern California", "S50" },
    { "Northern", "S55" },
    { "Pacific NW", "S60" },
    { "Southern California", "S65" },
    { "Southern", "S70" },
    { "Southwest", "S75" },
    { "Texas", "S80" },
    { "Unassigned", "SS00" }
  };

  /// <summary>
  /// Construct the correct URL from CLI args
  /// </summary>
  private static string BuildUSTARankingURL(CLIOptions options)
  {
    // Build the query string
    var queryParams = new Dictionary<string, string>()
    {
      { "ntrp-searchText", options.Name ?? "" },
      { "ntrp-matchFormat", options.Format ?? "SINGLES"},
      { "ntrp-rankListGender", options.Gender ?? "M" },
      { "ntrp-ntrpPlayerLevel", options.Level ?? "level_4_0" },
      { "ntrp-sectionCode", SECTION_CODES[options.Section ?? "Northern California"] }
    };

    var url = QueryHelpers.AddQueryString(USTA_BASE_URL, queryParams);

    // Workaround for weird # getting ignored in QueryHelpers
    url = url.Insert(USTA_BASE_URL.Count(), "#") + "#tab=ntrp";
    return url;
  }

  /// <summary>
  /// Setup silent headless chrome driver service
  /// </summary>
  private static ChromeDriver CreateChromeDriverService()
  {
    ChromeDriverService service = ChromeDriverService.CreateDefaultService();
    service.LogPath = "chromedriver.log";
    service.SuppressInitialDiagnosticInformation = true;
    service.HideCommandPromptWindow = true;

    ChromeOptions options = new ChromeOptions();

    options.PageLoadStrategy = PageLoadStrategy.Normal;

    // Trying to suppress all selenium logging but there's still a little bit of noise 😢
    options.AddArguments(
      "no-sandbox",
      "headless",
      "disable-gpu",
      "disable-logging",
      "disable-dev-shm-usage",
      "window-size=1920,1080",
      "disable-extensions",
      "log-level=OFF",
      "output=/dev/null"
    );

    var driver = new ChromeDriver(service, options);
    return driver;
  }

  /// <summary>
  /// Extracts the HTML element and returns a Player object
  /// </summary>
  private static Player ScrapePlayerRanking(WebDriver driver, string url, string name)
  {

    // Navigate to the URL and wait for the page to load
    driver.Navigate().GoToUrl(url);
    WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, PAGE_LOAD_TIMEOUT));
    wait.Until(d => d.FindElement(By.ClassName(HTML_ELEMENT_TARGET)));

    var elements = driver.FindElements(By.ClassName(HTML_ELEMENT_TARGET));

    if (elements[3].Text != name)
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
    // Parse command line arguments
    var options = Parser.Default.ParseArguments<CLIOptions>(args).Value
      ?? throw new Exception("Failed to parse command line arguments");

    // Construct the URL from cli args
    var url = BuildUSTARankingURL(options);

    // Create the chrome driver
    var driver = CreateChromeDriverService();

    try
    {
      // Scrape the player ranking
      var player = ScrapePlayerRanking(driver, url, options.Name ?? "");
      // Print out the player ranking as JSON or markdown
      if (options.JSON == true)
      {
        Console.WriteLine(player.ToJSON());
      }
      else
      {
        Console.WriteLine(player.ToMarkDown(options));
      }
    }
    catch (Exception)
    {
      Console.WriteLine($"No ranking found for {options.Name}");
    }
    finally
    {
      // Close the driver
      driver.Quit();
    }
  }
}

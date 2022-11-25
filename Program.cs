using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Newtonsoft.Json;
using Microsoft.AspNetCore.WebUtilities;
using CommandLine;

public class Program
{
  // Specific CSS class used by the USTA website, this may change in the future...
  private static string HTML_ELEMENT_TARGET = "v-grid-cell__content";
  // USTA rankings base URL also subject to change in the future...
  private static string USTA_BASE_URL = "https://www.usta.com/en/home/play/rankings.html";


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
      "--no-sandbox",
      "--headless",
      "--disable-gpu",
      "--disable-logging",
      "--disable-dev-shm-usage",
      "--window-size=1920,1080",
      "--disable-extensions",
      "--log-level=OFF",
      "--output=/dev/null"
    );

    var driver = new ChromeDriver(service, options);
    return driver;
  }

  /// <summary>
  /// Extracts the HTML element and returns a Player object
  /// </summary>
  private static Player ScrapePlayerRanking(WebDriver driver)
  {
    var elements = driver.FindElements(By.ClassName(HTML_ELEMENT_TARGET));

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

    // Navigate to the URL and wait for the page to load
    driver.Navigate().GoToUrl(url);
    WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 30));
    wait.Until(d => d.FindElement(By.ClassName(HTML_ELEMENT_TARGET)));

    // Scrape the player ranking
    var player = ScrapePlayerRanking(driver);

    // Close the driver
    driver.Quit();

    // Print out the player ranking as JSON or markdown
    if (options.JSON == true)
    {
      Console.WriteLine(JsonConvert.SerializeObject(player, Formatting.Indented));
    }
    else
    {
      // Print out the player ranking as markdown
      Console.WriteLine("## " + player.Name);

      Console.WriteLine("\n- National Rank: " + player.NationalRank);
      Console.WriteLine("- Section Rank: " + player.SectionRank);
      Console.WriteLine("- District Rank: " + player.DistrictRank);
    }
  }
}

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

  /// <summary>
  /// Construct the correct URL from CLI args
  /// </summary>
  private static string BuildUSTARankingURL(CLIOptions options)
  {
    var ustaBase = "https://www.usta.com/en/home/play/rankings.html";

    var queryParams = new Dictionary<string, string>()
    {
      { "ntrp-searchText", options.Name ?? "" },
      { "ntrp-matchFormat", options.Format ?? "SINGLES"},
      { "ntrp-rankListGender", options.Gender ?? "M" },
      { "ntrp-ntrpPlayerLevel", options.Level ?? "level_4_0" },
      { "ntrp-sectionCode", options.Section ?? "S50" }
    };

    var url = QueryHelpers.AddQueryString(ustaBase, queryParams);

    // Workaround for weird # getting ignored in QueryHelpers
    url = url.Insert(ustaBase.Count(), "#") + "#tab=ntrp";
    return url;
  }

  /// <summary>
  /// Setup silent headless chrome driver and wait for the page to load
  /// </summary>
  private static ChromeDriver CreateChromeDriverService()
  {
    ChromeDriverService service = ChromeDriverService.CreateDefaultService();
    service.LogPath = "chromedriver.log";
    service.SuppressInitialDiagnosticInformation = true;
    service.HideCommandPromptWindow = true;

    ChromeOptions options = new ChromeOptions();

    options.PageLoadStrategy = PageLoadStrategy.Normal;
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

    // Grab rankings and print them out
    var nationalRank = elements[0].Text;
    var sectionRank = elements[1].Text;
    var districtRank = elements[2].Text;
    var name = elements[3].Text;

    var player = new Player()
    {
      Name = elements[3].Text,
      NationalRank = int.Parse(elements[0].Text),
      SectionRank = int.Parse(elements[1].Text),
      DistrictRank = int.Parse(elements[2].Text)
    };
    return player;
  }

  /// <summary>
  /// Prints the player object to the console as JSON
  /// </summary>
  private static void PrintAsJSON(Player player)
  {
    var json = JsonConvert.SerializeObject(player, Formatting.Indented);
    Console.WriteLine(json);
  }

  /// <summary>
  /// Prints the player object to the console as a markdown list
  /// </summary>
  private static void PrintAsMarkdown(Player player)
  {
    Console.WriteLine("## " + player.Name);

    Console.WriteLine("\n- National Rank: " + player.NationalRank);
    Console.WriteLine("- Section Rank: " + player.SectionRank);
    Console.WriteLine("- District Rank: " + player.DistrictRank);
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

    // Print out the player ranking
    if (options.Output == "json")
    {
      PrintAsJSON(player);
    }
    else
    {
      PrintAsMarkdown(player);
    }
  }
}
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.AspNetCore.WebUtilities;

internal class Program
{
  // Specific CSS class used by the usta website, this may change in the future...
  private static string HTML_ELEMENT_TARGET = "v-grid-cell__content";

  // Construct the correct URL based on environment variables
  private static string BuildUSTARankingURL()
  {
    // Read in config values for appsettings.json to construct the URL
    var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", false)
                    .AddEnvironmentVariables()
                    .Build();

    var queryParams = config.GetSection("Query").Get<Dictionary<string, string>>();
    var ustaBase = "https://www.usta.com/en/home/play/rankings.html";
    var url = QueryHelpers.AddQueryString(ustaBase, queryParams);

    // Workaround for weird # getting ignored in QueryHelpers
    url = url.Insert(ustaBase.Count(), "#") + "#tab=ntrp";
    return url;
  }

  // Setup silent headless chrome driver and wait for the page to load
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

  // Extracts the HTML element and returns a Player object
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

  private static void Main(string[] args)
  {
    // Construct the URL
    var url = BuildUSTARankingURL();

    // Create the driver
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
    Console.WriteLine(JsonConvert.SerializeObject(player, Formatting.Indented));
  }
}
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.AspNetCore.WebUtilities;

// TODO setup github action to run this every week, get the results and update the value in my readme
// TODO Or create a badge that shows the current rank with link!!!

internal class Program
{
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

  // Setup quiet headless chrome driver and wait for the page to load
  private static FirefoxDriver CreateFirefoxDriverService()
  {
    FirefoxDriverService service = FirefoxDriverService.CreateDefaultService();
    service.LogLevel = FirefoxDriverLogLevel.Fatal;
    service.SuppressInitialDiagnosticInformation = true;
    service.HideCommandPromptWindow = true;

    FirefoxOptions options = new FirefoxOptions();

    options.PageLoadStrategy = PageLoadStrategy.Normal;
    // options.UnhandledPromptBehavior = UnhandledPromptBehavior.Accept;
    options.LogLevel = FirefoxDriverLogLevel.Fatal;

    options.AddArgument("--no-sandbox");
    options.AddArgument("--headless");
    options.AddArgument("--disable-gpu");
    options.AddArgument("--disable-logging");
    options.AddArgument("--log-level=3");
    options.AddArgument("--output=/dev/null");
    options.AddArgument("--window-size=1920,1080");
    options.AddArgument("--disable-extensions");

    var driver = new FirefoxDriver(service, options, new TimeSpan(0, 2, 0));
    return driver;
  }

  // Setup headless chrome driver and wait for the page to load
  private static ChromeDriver CreateChromeDriverService()
  {
    ChromeDriverService service = ChromeDriverService.CreateDefaultService();
    service.LogPath = "chromedriver.log";
    service.HideCommandPromptWindow = true;

    ChromeOptions options = new ChromeOptions();

    options.PageLoadStrategy = PageLoadStrategy.Eager;
    options.AddArgument("--no-sandbox");
    options.AddArgument("--headless");
    options.AddArgument("--disable-gpu");
    options.AddArgument("--disable-logging");
    options.AddArgument("--log-level=3");
    options.AddArgument("--output=/dev/null");
    options.AddArgument("--window-size=1920,1080");
    options.AddArgument("--disable-extensions");

    var driver = new ChromeDriver(service, options, new TimeSpan(0, 2, 0));
    return driver;
  }

  private static Player ScrapePlayerRanking(WebDriver driver)
  {
    // Get all the elements with the class name "cell__text" (single tr element)
    var elements = driver.FindElements(By.ClassName("cell__text"));

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
    // var driver = CreateChromeDriverService();
    var driver = CreateFirefoxDriverService();

    // Navigate to the URL and wait for the page to load
    driver.Navigate().GoToUrl(url);
    WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 30));
    driver.GetScreenshot().SaveAsFile("screenshot.png", ScreenshotImageFormat.Png);
    wait.Until(d => d.FindElement(By.ClassName("cell__text")));

    // Scrape the player ranking
    var player = ScrapePlayerRanking(driver);

    // Close the driver
    driver.Quit();

    // Print out the player ranking
    Console.WriteLine(JsonConvert.SerializeObject(player, Formatting.Indented));
  }
}
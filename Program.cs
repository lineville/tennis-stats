using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using Microsoft.Extensions.Configuration;
using System.Web;
using Newtonsoft.Json;

// TODO setup github action to run this every week, get the results and update the value in my readme
// TODO Or create a badge that shows the current rank with link!!!


static string BuildUSTARankingURL()
{
  // Read in config values for appsettings.json to construct the URL
  var config = new ConfigurationBuilder()
                  .AddJsonFile("appsettings.json", false)
                  .AddEnvironmentVariables()
                  .Build();

  var queryParams = config.GetSection("UserConfig").Get<Dictionary<string, string>>();

  // TODO -- it would be nice to just pass in the whole dictionary using QueryHelpers , but it omits the # after rankings.html ... so I have to manually add it
  var url = @$"https://www.usta.com/en/home/play/rankings.html#?
ntrp-matchFormat={queryParams["ntrp-matchFormat"]}&
ntrp-rankListGender={queryParams["ntrp-rankListGender"]}&
ntrp-ntrpPlayerLevel={queryParams["ntrp-ntrpPlayerLevel"]}&
ntrp-sectionCode={queryParams["ntrp-sectionCode"]}&
searchText={HttpUtility.UrlEncode(queryParams["searchText"])}#tab=ntrp";

  return url;
}

static FirefoxDriver CreateDriverService()
{
  // Setup headless chrome driver and wait for the page to load
  FirefoxDriverService service = FirefoxDriverService.CreateDefaultService();
  service.LogLevel = FirefoxDriverLogLevel.Fatal;
  service.SuppressInitialDiagnosticInformation = true;
  service.HideCommandPromptWindow = true;

  FirefoxOptions options = new FirefoxOptions();

  options.PageLoadStrategy = PageLoadStrategy.Normal;

  options.AddArgument("--window-size=1920,1080");
  options.AddArgument("--no-sandbox");
  options.AddArgument("--headless");
  options.AddArgument("--disable-gpu");
  options.AddArgument("--disable-crash-reporter");
  options.AddArgument("--disable-extensions");
  options.AddArgument("--disable-in-process-stack-traces");
  options.AddArgument("--disable-logging");
  options.AddArgument("--disable-dev-shm-usage");
  options.AddArgument("--log-level=3");
  options.AddArgument("--output=/dev/null");

  var driver = new FirefoxDriver(service, options);
  return driver;
}

static Player ScrapePlayerRanking(FirefoxDriver driver)
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

// Construct the URL
var url = BuildUSTARankingURL();

// Create the driver
var driver = CreateDriverService();

// Navigate to the URL and wait for the page to load
driver.Navigate().GoToUrl(url);
WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 1, 0));
wait.Until(d => d.FindElement(By.ClassName("cell__text")));

// Scrape the player ranking
var player = ScrapePlayerRanking(driver);
string jsonString = JsonConvert.SerializeObject(player, Formatting.Indented);

// Print out the player ranking
Console.WriteLine(jsonString);

// Close the driver
driver.Quit();

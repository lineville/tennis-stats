using OpenQA.Selenium;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support.UI;
using Microsoft.Extensions.Configuration;
using System.Web;

// Read in config values for appsettings.json to construct the URL
var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables()
                .Build();

var queryParams = config.GetSection("UserConfig").Get<Dictionary<string, string>>();
var url = $"https://www.usta.com/en/home/play/rankings.html#?ntrp-matchFormat={queryParams["ntrp-matchFormat"]}&ntrp-rankListGender={queryParams["ntrp-rankListGender"]}&ntrp-ntrpPlayerLevel={queryParams["ntrp-ntrpPlayerLevel"]}&ntrp-sectionCode={queryParams["ntrp-sectionCode"]}&searchText={HttpUtility.UrlEncode(queryParams["searchText"])}#tab=ntrp";

// Launch headless browser, go to url and wait for it to be ready
var driver = new SafariDriver();
driver.Navigate().GoToUrl(url);
WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 1, 0));

wait.Until(d => d.FindElement(By.ClassName("cell__text")));

// Get all the elements with the class name "cell__text" (single tr element)
var elements = driver.FindElements(By.ClassName("cell__text"));

// Grab rankings and print them out
var nationalRank = elements[0].Text;
var sectionRank = elements[1].Text;
var districtRank = elements[2].Text;
var name = elements[3].Text;

Console.WriteLine($"{name}\n-------------------------\n");
Console.WriteLine($"National Rank: #{nationalRank}");
Console.WriteLine($"Section Rank: #{sectionRank}");
Console.WriteLine($"District Rank: #{districtRank}");

driver.Quit();
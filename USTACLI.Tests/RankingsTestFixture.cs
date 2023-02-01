using Microsoft.Extensions.Configuration;
using OpenQA.Selenium.Chrome;

public class RankingsTestFixture : IDisposable
{
  public RankingsSettings Settings { get; set; }
  public IConfiguration Configuration { get; set; }
  public ChromeDriver ChromeDriver { get; set; }

  public RankingsTestFixture()
  {
    Settings = new RankingsSettings()
    {
      Name = "Liam Neville",
      Format = MatchFormat.SINGLES,
      Gender = Gender.M,
      Level = "4.0",
      Section = "Northern California"
    };

    Configuration = new ConfigurationBuilder()
      .SetBasePath(AppContext.BaseDirectory)
      .AddJsonFile("appsettings.Test.json", false, true)
      .Build();

    ChromeDriver = Driver.Create();

  }

  public void Dispose()
  {
    ChromeDriver.Quit();
    ChromeDriver.Dispose();
  }
}
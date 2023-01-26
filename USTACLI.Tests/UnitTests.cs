namespace USTACLI.Tests;

using Xunit;
using Microsoft.Extensions.Configuration;

public class UnitTests
{
    [Fact]
    public void TestBuildUSTARankingURL()
    {
        var options = new CLIOptions()
        {
            Name = "Liam Neville",
            Format = MatchFormat.SINGLES,
            Gender = Gender.M,
            Level = "4.0",
            Section = "Northern California"
        };

        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Test.json", false, true)
            .Build();

        var url = Program.BuildUSTARankingURL(options, config);
        Assert.Equal("https://www.usta.com/en/home/play/rankings.html#?ntrp-searchText=Liam%20Neville&ntrp-matchFormat=SINGLES&ntrp-rankListGender=M&ntrp-ntrpPlayerLevel=level_4_0&ntrp-sectionCode=S50#tab=ntrp", url);
    }

    [Fact]
    public void TestChromeDriverService()
    {
        var service = Program.CreateChromeDriverService();
        Assert.NotNull(service);
    }

    [Fact]
    public void TestScrapePlayerRanking()
    {
        var options = new CLIOptions()
        {
            Name = "Liam Neville",
            Format = MatchFormat.SINGLES,
            Gender = Gender.M,
            Level = "4.0",
            Section = "Northern California"
        };

        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Test.json", false, true)
            .Build();

        var driver = Program.CreateChromeDriverService();
        var url = Program.BuildUSTARankingURL(options, config);

        var player = Program.ScrapePlayerRanking(driver, url, config, options);

        Assert.NotNull(player);
    }
}
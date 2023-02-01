namespace USTACLI.Tests;

using Xunit;
using Microsoft.Extensions.Configuration;

public class RankingsTests
{
    [Fact]
    public void TestBuildUSTARankingURL()
    {
        var settings = new RankingsSettings()
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

        var url = GetRankingsCommand.BuildUSTARankingURL(settings, config);
        Assert.Equal("https://www.usta.com/en/home/play/rankings.html#?ntrp-searchText=Liam%20Neville&ntrp-matchFormat=SINGLES&ntrp-rankListGender=M&ntrp-ntrpPlayerLevel=level_4_0&ntrp-sectionCode=S50#tab=ntrp", url);
    }

    [Fact]
    public void TestChromeDriverService()
    {
        var service = GetRankingsCommand.CreateChromeDriverService();
        Assert.NotNull(service);
    }

    [Fact]
    public void TestScrapePlayerRanking()
    {
        var settings = new RankingsSettings()
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

        var driver = GetRankingsCommand.CreateChromeDriverService();
        var url = GetRankingsCommand.BuildUSTARankingURL(settings, config);

        var player = GetRankingsCommand.ScrapePlayerRanking(driver, url, config, settings);

        Assert.NotNull(player);
    }
}
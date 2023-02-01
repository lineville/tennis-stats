namespace USTACLI.Tests;

using Xunit;
using Microsoft.Extensions.Configuration;
using Spectre.Console.Cli;

public class RankingsTests : IClassFixture<RankingsTestFixture>
{
  private readonly RankingsTestFixture Fixture;

  public RankingsTests(RankingsTestFixture fixture)
  {
    Fixture = fixture;
  }

  [Fact]
  public void TestBuildUSTARankingURL()
  {
    var url = Utilities.BuildUSTARankingURL(Fixture.Settings, Fixture.Configuration, "get");

    Assert.Equal("https://www.usta.com/en/home/play/rankings.html#?ntrp-matchFormat=SINGLES&ntrp-rankListGender=M&ntrp-ntrpPlayerLevel=level_4_0&ntrp-sectionCode=S50&ntrp-searchText=Liam%20Neville#tab=ntrp", url);
  }

  [Fact]
  public void TestChromeDriverService()
  {
    Assert.NotNull(Fixture.ChromeDriver);
  }

  [Fact]
  public void TestGetPlayerRanking()
  {
    var player = GetRankingsCommand.ScrapePlayerRanking(Fixture.ChromeDriver, Fixture.Configuration, Fixture.Settings, "get");

    Assert.NotNull(player);
  }

  [Fact]
  public void TestListPlayerRanking()
  {
    var players = ListRankingsCommand.ScrapeRankings(Fixture.ChromeDriver, Fixture.Configuration, Fixture.Settings, "ListRankingsCommand");

    Assert.NotEmpty(players);
  }
}
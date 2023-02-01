namespace USTACLI.Tests;

using Xunit;
using Microsoft.Extensions.Configuration;

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

    var url = GetRankingsCommand.BuildUSTARankingURL(Fixture.Settings, Fixture.Configuration);
    Assert.Equal("https://www.usta.com/en/home/play/rankings.html#?ntrp-searchText=Liam%20Neville&ntrp-matchFormat=SINGLES&ntrp-rankListGender=M&ntrp-ntrpPlayerLevel=level_4_0&ntrp-sectionCode=S50#tab=ntrp", url);
  }

  [Fact]
  public void TestChromeDriverService()
  {
    Assert.NotNull(Fixture.ChromeDriver);
  }

  [Fact]
  public void TestGetPlayerRanking()
  {
    var url = GetRankingsCommand.BuildUSTARankingURL(Fixture.Settings, Fixture.Configuration);

    var player = GetRankingsCommand.ScrapePlayerRanking(Fixture.ChromeDriver, url, Fixture.Configuration, Fixture.Settings);

    Assert.NotNull(player);
  }

  [Fact]
  public void TestListPlayerRanking()
  {

    var url = ListRankingsCommand.BuildUSTARankingURL(Fixture.Settings, Fixture.Configuration);

    var players = ListRankingsCommand.ScrapeRankings(Fixture.ChromeDriver, url, Fixture.Configuration, Fixture.Settings);

    Assert.NotEmpty(players);
  }
}
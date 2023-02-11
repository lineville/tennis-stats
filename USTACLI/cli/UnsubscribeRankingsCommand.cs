using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Spectre.Console.Cli;
using StackExchange.Redis;

public class UnsubscribeRankingsCommand : Command<RankingsSettings>
{
#nullable disable
  public override int Execute(CommandContext context, RankingsSettings settings)
  {
#nullable enable
    // Load appsettings.json static data
    IConfiguration configuration = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: false)
      .Build();

    Utilities.InteractiveFallback(settings, configuration, context.Name);

    DeleteSubscriber(configuration, settings).GetAwaiter().GetResult();

    return 0;
  }

  public async Task DeleteSubscriber(IConfiguration configuration, RankingsSettings settings)
  {
    // Use Redis C# SDK to delete a user from the subscriber list
    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
    {
      User = configuration["REDIS_USER"],
      Password = configuration["REDIS_PASSWORD"],
      EndPoints = { configuration["REDIS_ENDPOINT"] ?? "localhost:6379" },
      AllowAdmin = true
    });

    var db = redis.GetDatabase();

    var key = $"{settings.Email}-{settings.Name}-{settings.Format}-{settings.Gender}-{settings.Level}-{settings.Section}";
    await db.KeyDeleteAsync(key);

    db.Multiplexer.Close();
  }
}
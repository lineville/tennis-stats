using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Spectre.Console.Cli;

public class SubscribeRankingsCommand : Spectre.Console.Cli.Command<RankingsSettings>
{
#nullable disable
  public override int Execute(CommandContext context, RankingsSettings settings)
  {
#nullable enable
    // Load appsettings.json static data
    IConfiguration configuration = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: false)
      .AddEnvironmentVariables()
      .Build();

    Utilities.InteractiveFallback(settings, configuration, context.Name);

    AddSubscriber(configuration, settings).GetAwaiter().GetResult();

    return 0;
  }

  public async Task AddSubscriber(IConfiguration configuration, RankingsSettings settings)
  {
    var connection = MongoClientSettings.FromConnectionString($"mongodb+srv://admin:{configuration["MONGO_PASSWORD"]}@prod.gngcbq9.mongodb.net/?retryWrites=true&w=majority");
    connection.ServerApi = new ServerApi(ServerApiVersion.V1);
    var client = new MongoClient(connection);
    var db = client.GetDatabase("usta-cli-subscribers");

    var subscribers = db.GetCollection<RankingsSettings>("subscribers");

    await subscribers.InsertOneAsync(settings);
  }
}
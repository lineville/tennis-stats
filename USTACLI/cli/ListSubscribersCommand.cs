using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using Spectre.Console.Cli;

public class ListSubscribersCommand : Command
{
#nullable disable
  public override int Execute(CommandContext context)
  {
#nullable enable
    var configuration = context.Data as IConfiguration
      ?? throw new Exception("Failed to load configuration from appsettings.json");

    GetSubscribers(configuration).GetAwaiter().GetResult();
    return 0;
  }

  public async Task GetSubscribers(IConfiguration configuration)
  {
    var connection = MongoClientSettings.FromConnectionString($"mongodb+srv://admin:{configuration["MONGO_PASSWORD"]}@prod.gngcbq9.mongodb.net/?retryWrites=true&w=majority");
    connection.ServerApi = new ServerApi(ServerApiVersion.V1);
    var client = new MongoClient(connection);
    var db = client.GetDatabase("usta-cli-subscribers");

    var subscribers = db.GetCollection<RankingsSettings>("subscribers");

    var filter = Builders<RankingsSettings>.Filter.Empty;

    var docs = await subscribers.Find(filter).ToListAsync();

    Console.WriteLine(JsonConvert.SerializeObject(docs, Formatting.None));
  }
}
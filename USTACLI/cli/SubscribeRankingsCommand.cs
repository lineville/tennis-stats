using Jering.Javascript.NodeJS;
using Microsoft.Extensions.Configuration;
using Spectre.Console.Cli;

public class SubscribeRankingsCommand : Command<RankingsSettings>
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

    var res = AddSubscriber().GetAwaiter().GetResult();

    return 0;
  }

  public async Task<string?> AddSubscriber()
  {
    // TODO just shell out to nodejs instead...
    string javascriptModule = @"
    
    module.exports = async (callback) => {
      // require('cross-fetch/polyfill')
      const PocketBase = require('pocketbase/cjs')
      
      const pb = new PocketBase('https://usta-cli-subscribers.pockethost.io')
      await pb.admins.authWithPassword('liamgneville@gmail.com', '${{ secrets.POCKETHOST_PASSWORD }}')

      const data = {
        'email': 'test@example.com',
        'emailVisibility': true,
        'password': '12345678',
        'passwordConfirm': '12345678',
        'name': 'test',
        'format': 'test',
        'gender': 'test',
        'level': 'test',
        'section': 'test'
      };

      const record = await pb.collection('users').create(data);

      // (optional) send an email verification request
      await pb.collection('users').requestVerification('test@example.com');
      
      const result = 'Hello World';
      callback(null, result);
    }";

    // Invoke javascript
    var result = await StaticNodeJSService.InvokeFromStringAsync<string>(javascriptModule);
    return result;
  }
}
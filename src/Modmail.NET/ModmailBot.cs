using DSharpPlus;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;
using Modmail.NET.Features.UserInfo;
using Serilog;

namespace Modmail.NET;

public class ModmailBot
{
  public ModmailBot(DiscordClient client) {
    Client = client;
  }

  public bool Connected => Client.AllShardsConnected;
  public DiscordClient Client { get; }


  public async Task StartAsync() {
    Log.Information("Starting bot");

    await Client.ConnectAsync(Const.DiscordActivity);

    while (!Client.AllShardsConnected) await Task.Delay(5);


    var scope = Client.ServiceProvider.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    var options = scope.ServiceProvider.GetRequiredService<IOptions<BotConfig>>();
    await sender.Send(new UpdateDiscordUserCommand(Client.CurrentUser));


    var guildJoined = Client.Guilds.TryGetValue(options.Value.MainServerId, out var guild);
    if (!guildJoined) throw new NotJoinedMainServerException();

    var isSetup = await sender.Send(new CheckAnyGuildSetupQuery());
    if (!isSetup) {
      Log.Information($"[{nameof(ModmailBot)}]{nameof(StartAsync)} Setting up main server");
      try {
        await sender.Send(new ProcessGuildSetupCommand(Client.CurrentUser.Id, guild));
        Log.Information($"[{nameof(ModmailBot)}]{nameof(StartAsync)} main server setup complete");
      }
      catch (MainServerAlreadySetupException) {
        Log.Information($"[{nameof(ModmailBot)}]{nameof(StartAsync)} main server already setup");
      }
      catch (Exception ex) {
        Log.Fatal(ex, $"[{nameof(ModmailBot)}]{nameof(StartAsync)} main server setup exception");
        throw;
      }
    }
  }

  public async Task StopAsync() {
    Log.Information("Stopping bot");
    await Client.DisconnectAsync();
    Client.Dispose();
  }
}
using DSharpPlus;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Static;
using Modmail.NET.Features.Server.Commands;
using Modmail.NET.Features.Server.Queries;
using Modmail.NET.Features.Teams.Commands;
using Modmail.NET.Features.User.Commands;
using Modmail.NET.Language;
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

    await sender.Send(new SyncSuperUserTeamUsersCommand(Client.CurrentUser.Id));

    var guildJoined = Client.Guilds.TryGetValue(options.Value.MainServerId, out var guild);
    if (!guildJoined || guild is null) throw new ModmailBotException(Lang.NotJoinedMainServer);

    var isSetup = await sender.Send(new CheckSetupQuery());
    if (!isSetup) {
      Log.Information($"[{nameof(ModmailBot)}]{nameof(StartAsync)} Setting up main server");
      try {
        await sender.Send(new ProcessSetupCommand(Client.CurrentUser.Id, guild));
        Log.Information($"[{nameof(ModmailBot)}]{nameof(StartAsync)} main server setup complete");
      }
      catch (ModmailBotException) {
        Log.Information($"[{nameof(ModmailBot)}]{nameof(StartAsync)} main server already setup");
      }
      catch (Exception ex) {
        Log.Fatal(ex, $"[{nameof(ModmailBot)}]{nameof(StartAsync)} main server setup exception");
        throw;
      }
    }


    _ = await sender.Send(new ProcessCreateOrUpdateLogChannelCommand(Client.CurrentUser.Id, guild));
  }

  public async Task StopAsync() {
    Log.Information("Stopping bot");
    await Client.DisconnectAsync();
    Client.Dispose();
  }
}
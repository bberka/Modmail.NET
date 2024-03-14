using System.Diagnostics;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Commands;
using Modmail.NET.Common;
using Modmail.NET.Database;
using Modmail.NET.Events;
using Modmail.NET.Static;
using Serilog;
using Serilog.Extensions.Logging;

namespace Modmail.NET;

public class ModmailBot
{
  public ModmailBot() {
    try
    {
      DotEnv.Init(); // Load the .env file
      _ = EnvContainer.This; // Initialize the environment container
    }
    catch (Exception ex)
    {
      Log.Error(ex, "Failed to initialize environment variables");
      throw;
    }
    SLogConfigMgr.Configure();
    if (!EnvContainer.This.IsInitialized)
      throw new Exception("EnvContainer is not initialized.");

    if (EnvContainer.This.Environment == EnvironmentType.Development)
      Log.Information("Running in development mode");
  }

  private DiscordClient _discordClient;

  public async Task StartAsync() {
    // Start the bot
    _discordClient = new DiscordClient(new DiscordConfiguration() {
      Token = EnvContainer.This.BotToken,
      AutoReconnect = true,
      TokenType = TokenType.Bot,
      Intents = DiscordIntents.All,
      HttpTimeout = TimeSpan.FromSeconds(10),
      LogUnknownEvents = false,
      LoggerFactory = new SerilogLoggerFactory(Log.Logger),
    });
    _discordClient.Heartbeated += BaseEvents.OnHeartbeat;
    _discordClient.Ready += BaseEvents.OnReady;
    _discordClient.ClientErrored += BaseEvents.OnClientError;
    _discordClient.SocketErrored += BaseEvents.OnSocketError;

    _discordClient.MessageCreated += MessageEvents.OnMessageCreated;
    _discordClient.MessageDeleted += MessageEvents.OnMessageDeleted;
    _discordClient.MessageUpdated += MessageEvents.OnMessageUpdated;

    var commandsconfig = new CommandsNextConfiguration {
      StringPrefixes = new[] {EnvContainer.This.BotPrefix},
      EnableDms = true,
      EnableMentionPrefix = true,
      DmHelp = false,
      CaseSensitive = false,
      IgnoreExtraArguments = true,
    };
    var slash = _discordClient.UseSlashCommands();
    var commands = _discordClient.UseCommandsNext(commandsconfig);
    commands.RegisterCommands<AdminCommands>();

    
    await _discordClient.ConnectAsync();
    
    await Task.Delay(-1);
  }

  private async Task StopAsync() {
    await _discordClient.DisconnectAsync();
  }

  
}
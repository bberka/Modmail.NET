using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using DSharpPlus.Extensions;
using Modmail.NET.Commands;
using Modmail.NET.Commands.Slash;
using Serilog;

namespace Modmail.NET.Web.Blazor.Dependency;

public static class DiscordBotDependency
{
  public static void Configure(WebApplicationBuilder builder) {
    builder.Services.AddDiscordClient(builder.Configuration.GetValue<string>("Bot:BotToken") ?? throw new Exception("BotToken is empty"),
                                      DiscordIntents.MessageContents
                                      | DiscordIntents.Guilds
                                      | DiscordIntents.GuildMessages
                                      | DiscordIntents.GuildMembers
                                      | DiscordIntents.DirectMessages
                                      | DiscordIntents.GuildMessageReactions
                                      | DiscordIntents.DirectMessageReactions);

    builder.Services.AddCommandsExtension((_, extension) => {
      extension.AddCommands<ModmailCommands>();
      extension.AddCommands<BlacklistSlashCommands>();
      extension.AddCommands<TicketSlashCommands>();

      extension.AddChecks(typeof(ModmailBotProjectMarker).Assembly);
      TextCommandProcessor textCommandProcessor = new(new TextCommandConfiguration {
        PrefixResolver = new DefaultPrefixResolver(true, builder.Configuration.GetValue<string>("Bot:BotPrefix") ?? throw new Exception("BotPrefix is null")).ResolvePrefixAsync,
        IgnoreBots = true,
        SuppressMissingMessageContentIntentWarning = false,
        CommandNameComparer = StringComparer.InvariantCultureIgnoreCase
      });

      extension.AddProcessor(textCommandProcessor);

      extension.CommandErrored += (_, args) => {
        Log.Error(args.Exception, "Error executing command {Command} {@Args}", args.Context.Command.FullName, args.Context.Arguments);
        return Task.CompletedTask;
      };
    });


    builder.Services.ConfigureEventHandlers(eventHandlingBuilder => {
      eventHandlingBuilder.HandleMessageCreated(ModmailEventHandlers.OnMessageCreated);
      eventHandlingBuilder.HandleChannelDeleted(ModmailEventHandlers.OnChannelDeleted);

      eventHandlingBuilder.HandleMessageReactionAdded(ModmailEventHandlers.OnMessageReactionAdded);
      eventHandlingBuilder.HandleMessageReactionRemoved(ModmailEventHandlers.OnMessageReactionRemoved);

      //TODO: investigate the need to implement handling of other reaction events
      // eventHandlingBuilder.HandleMessageReactionsCleared();
      // eventHandlingBuilder.HandleMessageReactionRemovedEmoji();

      eventHandlingBuilder.HandleMessageDeleted(ModmailEventHandlers.OnMessageDeleted);
      eventHandlingBuilder.HandleMessageUpdated(ModmailEventHandlers.OnMessageUpdated);

      eventHandlingBuilder.HandleInteractionCreated(ModmailEventHandlers.InteractionCreated);
      eventHandlingBuilder.HandleComponentInteractionCreated(ModmailEventHandlers.ComponentInteractionCreated);
      eventHandlingBuilder.HandleModalSubmitted(ModmailEventHandlers.ModalSubmitted);

      eventHandlingBuilder.HandleGuildMemberAdded(ModmailEventHandlers.OnGuildMemberAdded);
      eventHandlingBuilder.HandleGuildMemberRemoved(ModmailEventHandlers.OnGuildMemberRemoved);
      eventHandlingBuilder.HandleGuildBanAdded(ModmailEventHandlers.OnGuildBanAdded);
      eventHandlingBuilder.HandleGuildBanRemoved(ModmailEventHandlers.OnGuildBanRemoved);

      eventHandlingBuilder.HandleUserUpdated(ModmailEventHandlers.OnUserUpdated);
      eventHandlingBuilder.HandleUserSettingsUpdated(ModmailEventHandlers.OnUserSettingsUpdated);
    });
  }
}
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using DSharpPlus.Extensions;
using Modmail.NET.Features.DiscordBot.Events;
using Modmail.NET.Features.DiscordCommands.Handlers;
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
      extension.AddCommands<BlacklistSlashCommands>();
      extension.AddCommands<TicketSlashCommands>();
      extension.AddCommands<TagSlashCommands>();

      extension.AddChecks(typeof(ModmailBot).Assembly);
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
      eventHandlingBuilder.HandleMessageCreated(OnMessageCreatedEvent.OnMessageCreated);
      eventHandlingBuilder.HandleMessageDeleted(OnMessageDeletedEvent.OnMessageDeleted);
      eventHandlingBuilder.HandleMessageUpdated(OnMessageUpdatedEvent.OnMessageUpdated);

      eventHandlingBuilder.HandleMessageReactionAdded(OnMessageReactionAddedEvent.OnMessageReactionAdded);
      eventHandlingBuilder.HandleMessageReactionRemoved(OnMessageReactionRemovedEvent.OnMessageReactionRemoved);

      eventHandlingBuilder.HandleChannelDeleted(OnChannelDeletedEvent.OnChannelDeleted);

      eventHandlingBuilder.HandleComponentInteractionCreated(ComponentInteractionCreatedEvent.ComponentInteractionCreated);

      eventHandlingBuilder.HandleModalSubmitted(ModalSubmittedEvent.ModalSubmitted);

      //TODO: investigate the need to implement handling of other reaction events
      // eventHandlingBuilder.HandleMessageReactionsCleared();
      // eventHandlingBuilder.HandleMessageReactionRemovedEmoji();

      //User update
      eventHandlingBuilder.HandleInteractionCreated(UserUpdateEvents.InteractionCreated);
      eventHandlingBuilder.HandleGuildMemberAdded(UserUpdateEvents.OnGuildMemberAdded);
      eventHandlingBuilder.HandleGuildMemberRemoved(UserUpdateEvents.OnGuildMemberRemoved);
      eventHandlingBuilder.HandleGuildBanAdded(UserUpdateEvents.OnGuildBanAdded);
      eventHandlingBuilder.HandleGuildBanRemoved(UserUpdateEvents.OnGuildBanRemoved);
      eventHandlingBuilder.HandleUserUpdated(UserUpdateEvents.OnUserUpdated);
      eventHandlingBuilder.HandleUserSettingsUpdated(UserUpdateEvents.OnUserSettingsUpdated);
    });
  }
}
using DSharpPlus.Entities;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Extensions;
using Serilog;

namespace Modmail.NET.Commands.Slash;

[PerformanceLoggerAspect]
[SlashCommandGroup("blacklist", "Blacklist management commands.")]
[RequirePermissionLevelOrHigherForSlash(TeamPermissionLevel.Moderator)]
[UpdateUserInformationForSlash]
public class BlacklistSlashCommands : ApplicationCommandModule
{
    [SlashCommand("add", "Add a user to the blacklist.")]
    public async Task Add(
        InteractionContext ctx,
        [Option("user", "The user to blacklist.")] DiscordUser user,
        [Option("reason", "The reason for blacklisting.")] string reason = "No reason provided."
    )
    {
        const string logMessage = $"[{nameof(BlacklistSlashCommands)}]{nameof(Add)}({{ContextUserId}},{{UserId}},{{Reason}})";
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AsEphemeral());
        try
        {
            await DiscordUserInfo.AddOrUpdateAsync(user);
            await TicketBlacklist.ProcessAddUserToBlacklist(user.Id, reason, ctx.User.Id);
            await ctx.EditResponseAsync(Webhooks.Success(LangKeys.USER_BLACKLISTED.GetTranslation()));
            Log.Information(logMessage, ctx.User.Id, user.Id, reason);
        }
        catch (BotExceptionBase ex)
        {
            Log.Warning(ex, logMessage, ctx.User.Id, user.Id, reason);
            await ctx.EditResponseAsync(ex.ToWebhookResponse());
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, logMessage, ctx.User.Id, user.Id, reason);
            await ctx.EditResponseAsync(ex.ToWebhookResponse());
        }
    }

    [SlashCommand("remove", "Remove a user from the blacklist.")]
    public async Task Remove(InteractionContext ctx, [Option("user", "The user to remove from the blacklist.")] DiscordUser user)
    {
        const string logMessage = $"[{nameof(BlacklistSlashCommands)}]{nameof(Remove)}({{ContextUserId}},{{UserId}})";
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AsEphemeral());
        try
        {
            await DiscordUserInfo.AddOrUpdateAsync(user);
            var ticketBlacklist = await TicketBlacklist.GetAsync(user.Id);
            await ticketBlacklist.ProcessRemoveUserFromBlacklist(ctx.User.Id, user.Id);
            Log.Information(logMessage, ctx.User.Id, user.Id);
            await ctx.EditResponseAsync(Webhooks.Success(LangKeys.USER_BLACKLISTED.GetTranslation()));
        }
        catch (BotExceptionBase ex)
        {
            await ctx.EditResponseAsync(ex.ToWebhookResponse());
            Log.Warning(ex, logMessage, ctx.User.Id, user.Id);
        }
        catch (Exception ex)
        {
            await ctx.EditResponseAsync(ex.ToWebhookResponse());
            Log.Fatal(ex, logMessage, ctx.User.Id, user.Id);
        }
    }

    [SlashCommand("status", "Check if a user is blacklisted.")]
    public async Task Status(InteractionContext ctx, [Option("user", "The user to check.")] DiscordUser user)
    {
        const string logMessage = $"[{nameof(BlacklistSlashCommands)}]{nameof(Status)}({{ContextUserId}},{{UserId}})";
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AsEphemeral());
        try
        {
            var isBlocked = await TicketBlacklist.IsBlacklistedAsync(user.Id);
            await ctx.EditResponseAsync(Webhooks.Info(LangKeys.USER_BLACKLIST_STATUS.GetTranslation(),
                isBlocked ? LangKeys.USER_IS_BLACKLISTED.GetTranslation() : LangKeys.USER_IS_NOT_BLACKLISTED.GetTranslation()));
            Log.Information(logMessage, ctx.User.Id, user.Id);
        }
        catch (BotExceptionBase ex)
        {
            await ctx.EditResponseAsync(ex.ToWebhookResponse());
            Log.Warning(ex, logMessage, ctx.User.Id, user.Id);
        }
        catch (Exception ex)
        {
            await ctx.EditResponseAsync(ex.ToWebhookResponse());
            Log.Fatal(ex, logMessage, ctx.User.Id, user.Id);
        }
    }
}
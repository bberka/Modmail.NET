using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Common.Aspects;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Database;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.Blacklist.Commands;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;
using Modmail.NET.Features.User.Commands;
using Modmail.NET.Language;
using Serilog;

namespace Modmail.NET.Features.DiscordCommands.Handlers;

[PerformanceLoggerAspect]
[Command("blacklist")]
[UpdateUserInformation]
public class BlacklistSlashCommands
{
    private readonly ISender _sender;

    public BlacklistSlashCommands(ISender sender)
    {
        _sender = sender;
    }

    [Command("add")]
    [Description("Adds a user to the blacklist")]
    [SlashCommandTypes(DiscordApplicationCommandType.SlashCommand)]
    [RequireModmailPermission(nameof(AuthPolicy.ManageBlacklist))]
    public async Task Add(
        SlashCommandContext ctx,
        [Parameter("user")] [Description("The user to blacklist")] DiscordUser user,
        [Parameter("reason")] [Description("The reason for blacklisting")] string reason = "No reason provided."
    )
    {
        const string logMessage = $"[{nameof(BlacklistSlashCommands)}]{nameof(Add)}({{ContextUserId}},{{UserId}},{{Reason}})";
        await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AsEphemeral());
        try
        {
            await _sender.Send(new UpdateDiscordUserCommand(user));
            await _sender.Send(new ProcessAddUserToBlacklistCommand(ctx.User.Id, user.Id, reason));
            await ctx.EditResponseAsync(ModmailWebhooks.Success(Lang.UserBlacklisted.Translate()));
            Log.Information(logMessage, ctx.User.Id, user.Id, reason);
        }
        catch (ModmailBotException ex)
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

    [Command("remove")]
    [Description("Removes a user from the blacklist.")]
    [SlashCommandTypes(DiscordApplicationCommandType.SlashCommand)]
    [RequireModmailPermission(nameof(AuthPolicy.ManageBlacklist))]
    public async Task Remove(SlashCommandContext ctx, [Parameter("user")] [Description("The user to remove from the blacklist.")] DiscordUser user)
    {
        const string logMessage = $"[{nameof(BlacklistSlashCommands)}]{nameof(Remove)}({{ContextUserId}},{{UserId}})";
        await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AsEphemeral());
        try
        {
            await _sender.Send(new UpdateDiscordUserCommand(user));
            await _sender.Send(new ProcessRemoveUserFromBlacklistCommand(user.Id, ctx.User.Id));
            Log.Information(logMessage, ctx.User.Id, user.Id);
            await ctx.EditResponseAsync(ModmailWebhooks.Success(Lang.UserBlacklisted.Translate()));
        }
        catch (ModmailBotException ex)
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

    [Command("status")]
    [Description("Check if a user is blacklisted.")]
    [RequireModmailPermission]
    public async Task Status(SlashCommandContext ctx, [Parameter("user")] [Description("The user to check.")] DiscordUser user)
    {
        const string logMessage = $"[{nameof(BlacklistSlashCommands)}]{nameof(Status)}({{ContextUserId}},{{UserId}})";
        await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AsEphemeral());
        try
        {
            var dbContext = ctx.ServiceProvider.GetRequiredService<ModmailDbContext>();
            var isBlocked = await dbContext.Blacklists.FilterByUserId(user.Id)
                .AnyAsync();
            await ctx.EditResponseAsync(ModmailWebhooks.Info(Lang.UserBlacklistStatus.Translate(),
                isBlocked ? Lang.UserIsBlacklisted.Translate() : Lang.UserIsNotBlacklisted.Translate()));
            Log.Information(logMessage, ctx.User.Id, user.Id);
        }
        catch (ModmailBotException ex)
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
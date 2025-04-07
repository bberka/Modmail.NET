using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Common.Aspects;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Static;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;
using Modmail.NET.Features.Guild.Commands;
using Modmail.NET.Language;
using Serilog;

namespace Modmail.NET.Features.DiscordCommands.Handlers;

[PerformanceLoggerAspect]
[RequireGuild]
[UpdateUserInformation]
[Command("modmail")]
[RequireApplicationOwner]
[RequirePermissions(DiscordPermission.Administrator)]
public class ModmailCommands
{
  private readonly ISender _sender;

  public ModmailCommands(ISender sender) {
    _sender = sender;
  }

  [Command("setup")]
  [Description("Setup the modmail bot, can only be used by the bot owner and administrator.")]
  public async Task Setup(CommandContext ctx) {
    const string logMessage = $"[{nameof(ModmailCommands)}]{nameof(Setup)}({{ContextUserId}})";

    try {
      await _sender.Send(new ProcessGuildSetupCommand(ctx.User.Id, ctx.Guild));
      await ctx.RespondAsync(ModmailEmbeds.Success(LangKeys.ServerSetupComplete.GetTranslation()));
      Log.Information(logMessage,
                      ctx.User.Id);
    }
    catch (ModmailBotException ex) {
      await ctx.RespondAsync(ex.ToEmbedResponse());
      Log.Warning(ex,
                  logMessage,
                  ctx.User.Id);
    }
    catch (Exception ex) {
      await ctx.RespondAsync(ex.ToEmbedResponse());
      Log.Fatal(ex,
                logMessage,
                ctx.User.Id);
    }
  }
}
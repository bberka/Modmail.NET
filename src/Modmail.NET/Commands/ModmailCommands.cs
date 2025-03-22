using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Aspects;
using Modmail.NET.Checks.Attributes;
using Modmail.NET.Exceptions;
using Modmail.NET.Extensions;
using Modmail.NET.Features.Guild;
using Serilog;

namespace Modmail.NET.Commands;

[PerformanceLoggerAspect]
[RequireGuild]
[UpdateUserInformation]
[Command("modmail")]
[RequireApplicationOwner]
[RequirePermissions(DiscordPermission.Administrator)]
public sealed class ModmailCommands 
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
      await _sender.Send(new ProcessGuildSetupCommand(ctx.Guild));
      await ctx.RespondAsync(Embeds.Success(LangKeys.SERVER_SETUP_COMPLETE.GetTranslation()));
      Log.Information(logMessage,
                      ctx.User.Id);
    }
    catch (BotExceptionBase ex) {
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
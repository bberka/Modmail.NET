using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Modmail.NET.Aspects;
using Modmail.NET.Attributes;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Extensions;
using Serilog;

namespace Modmail.NET.Commands;

[PerformanceLoggerAspect]
[RequireGuild]
[UpdateUserInformationForCommand]
[Group("modmail")]
[RequireOwner]
[RequirePermissions(Permissions.Administrator)]
public sealed class ModmailCommands : BaseCommandModule
{
  [Command("setup")]
  [Description("Setup the modmail bot, can only be used by the bot owner and administrator.")]
  [GroupCommand]
  public async Task Setup(CommandContext ctx) {
    const string logMessage = $"[{nameof(ModmailCommands)}]{nameof(Setup)}({{ContextUserId}})";

    try {
      await GuildOption.ProcessSetupAsync(ctx.Guild);
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
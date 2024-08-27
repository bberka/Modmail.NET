// using DSharpPlus;
// using DSharpPlus.CommandsNext;
// using DSharpPlus.CommandsNext.Attributes;
// using DSharpPlus.Entities;
// using DSharpPlus.SlashCommands;
// using Modmail.NET.Aspects;
// using Modmail.NET.Attributes;
// using Modmail.NET.Commands.Slash;
// using Modmail.NET.Entities;
// using Modmail.NET.Exceptions;
// using Modmail.NET.Extensions;
// using Serilog;
//
// namespace Modmail.NET.Commands;
//
// [PerformanceLoggerAspect]
// [UpdateUserInformationForCommand]
// [Group("blacklist")]
// [RequirePermissions(Permissions.Administrator)]
// [RequireMainServerForCommand]
// public class BlacklistCommands : BaseCommandModule
// {
//   [Command("add")]
//   [Description("Add a user to the blacklist.")]
//   [GroupCommand]
//   public async Task Add(CommandContext ctx,
//                         [Option("user", "The user to blacklist.")]
//                         DiscordUser user,
//                         [Option("notify-user", "Whether to notify the user about the blacklist.")]
//                         bool notifyUser = true,
//                         [Option("reason", "The reason for blacklisting.")]
//                         string reason = "No reason provided."
//   ) {
//     const string logMessage = $"[{nameof(BlacklistSlashCommands)}]{nameof(Add)}({{ContextUserId}},{{UserId}},{{NotifyUser}},{{Reason}})";
//     try {
//       await DiscordUserInfo.AddOrUpdateAsync(user);
//       await TicketBlacklist.ProcessAddUserToBlacklist(ctx.User.Id, user.Id, reason, notifyUser);
//       await ctx.RespondAsync(Embeds.Success(LangKeys.USER_BLACKLISTED.GetTranslation()));
//       Log.Information(logMessage,
//                       ctx.User.Id,
//                       user.Id,
//                       notifyUser,
//                       reason);
//     }
//     catch (BotExceptionBase ex) {
//       Log.Warning(ex,
//                   logMessage,
//                   ctx.User.Id,
//                   user.Id,
//                   notifyUser,
//                   reason);
//       await ctx.RespondAsync(ex.ToEmbedResponse());
//     }
//     catch (Exception ex) {
//       Log.Fatal(ex,
//                 logMessage,
//                 ctx.User.Id,
//                 user.Id,
//                 notifyUser,
//                 reason);
//       await ctx.RespondAsync(ex.ToEmbedResponse());
//     }
//   }
//
//   
//   [Command("add")]
//   [Description("Add a user to the blacklist.")]
//   [GroupCommand]
//   public async Task Add(CommandContext ctx,
//                         [Option("user", "The user to blacklist.")]
//                         DiscordUser user,
//                         [Option("notify-user", "Whether to notify the user about the blacklist.")]
//                         bool notifyUser = true,
//                         [Option("reason", "The reason for blacklisting.")]
//                         string reason = "No reason provided."
//   ) {
//     const string logMessage = $"[{nameof(BlacklistSlashCommands)}]{nameof(Add)}({{ContextUserId}},{{UserId}},{{NotifyUser}},{{Reason}})";
//     try {
//       await DiscordUserInfo.AddOrUpdateAsync(user);
//       await TicketBlacklist.ProcessAddUserToBlacklist(ctx.User.Id, user.Id, reason, notifyUser);
//       await ctx.RespondAsync(Embeds.Success(LangKeys.USER_BLACKLISTED.GetTranslation()));
//       Log.Information(logMessage,
//                       ctx.User.Id,
//                       user.Id,
//                       notifyUser,
//                       reason);
//     }
//     catch (BotExceptionBase ex) {
//       Log.Warning(ex,
//                   logMessage,
//                   ctx.User.Id,
//                   user.Id,
//                   notifyUser,
//                   reason);
//       await ctx.RespondAsync(ex.ToEmbedResponse());
//     }
//     catch (Exception ex) {
//       Log.Fatal(ex,
//                 logMessage,
//                 ctx.User.Id,
//                 user.Id,
//                 notifyUser,
//                 reason);
//       await ctx.RespondAsync(ex.ToEmbedResponse());
//     }
//   }
// }

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Aspects;
using Modmail.NET.Attributes;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Extensions;
using Modmail.NET.Providers;
using Modmail.NET.Static;
using Serilog;

namespace Modmail.NET.Commands;

[PerformanceLoggerAspect(ThresholdMs = 3000)]
[SlashCommandGroup("team", "Team management commands.")]
[RequirePermissionLevelOrHigher(TeamPermissionLevel.Admin)]
[UpdateUserInformation]
[RequireMainServer]
public class TeamSlashCommands : ApplicationCommandModule
{
  [SlashCommand("list", "List all teams.")]
  public async Task ListTeams(InteractionContext ctx) {
    const string logMessage = $"[{nameof(TeamSlashCommands)}]{nameof(ListTeams)}({{ContextUserId}})";
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());

    try {
      var teams = await GuildTeam.GetAllAsync();
      await ctx.Interaction.EditOriginalResponseAsync(CommandResponses.ListTeams(ctx.Guild, teams));
      Log.Information(logMessage, ctx.User.Id);
    }
    catch (BotExceptionBase ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex, logMessage, ctx.User.Id);
    }
    catch (Exception ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex, logMessage, ctx.User.Id);
    }
  }

  [SlashCommand("create", "Create a new team.")]
  public async Task CreateTeam(InteractionContext ctx,
                               [Option("teamName", "Team name")] string teamName,
                               [Option("permissionLevel", "Permission level")]
                               TeamPermissionLevel permissionLevel,
                               [Option("ping-on-new-ticket", "Ping on new ticket")]
                               bool pingOnNewTicket = false,
                               [Option("ping-on-ticket-message", "Ping on ticket message")]
                               bool pingOnTicketMessage = false
  ) {
    const string logMessage = $"[{nameof(TeamSlashCommands)}]{nameof(CreateTeam)}({{ContextUserId}},{{TeamName}},{{PermissionLevel}})";
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      await GuildTeam.ProcessCreateTeamAsync(teamName, permissionLevel, pingOnNewTicket, pingOnTicketMessage);
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.TEAM_CREATED_SUCCESSFULLY));
      Log.Information(logMessage, ctx.User.Id, teamName, permissionLevel);
    }
    catch (BotExceptionBase ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex, logMessage, ctx.User.Id, teamName, permissionLevel);
    }
    catch (Exception ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex, logMessage, ctx.User.Id, teamName, permissionLevel);
    }
  }

  [SlashCommand("update", "Update an existing team.")]
  public async Task UpdateTeam(InteractionContext ctx,
                               [Autocomplete(typeof(TeamProvider))] [Option("teamName", "Team name")]
                               string teamName,
                               [Option("is-enabled", "Is the team enabled")]
                               bool isEnabled,
                               [Option("permissionLevel", "Permission level")]
                               TeamPermissionLevel? permissionLevel = null,
                               [Option("ping-on-new-ticket", "Ping on new ticket")]
                               bool? pingOnNewTicket = null,
                               [Option("ping-on-ticket-message", "Ping on ticket message")]
                               bool? pingOnTicketMessage = false
  ) {
    const string logMessage = $"[{nameof(TeamSlashCommands)}]{nameof(UpdateTeam)}({{ContextUserId}},{{TeamName}},{{PermissionLevel}},{{PingOnNewTicket}},{{PingOnTicketMessage}},{{IsEnabled}})";
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var team = await GuildTeam.GetByNameAsync(teamName);
      await team.ProcessUpdateTeamAsync(ctx.Guild.Id,
                                        teamName,
                                        permissionLevel,
                                        pingOnNewTicket,
                                        pingOnTicketMessage,
                                        isEnabled);
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.TEAM_CREATED_SUCCESSFULLY));
      Log.Information(logMessage,
                      ctx.User.Id,
                      teamName,
                      permissionLevel,
                      pingOnNewTicket,
                      pingOnTicketMessage,
                      isEnabled);
    }
    catch (BotExceptionBase ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex,
                  logMessage,
                  ctx.User.Id,
                  teamName,
                  permissionLevel,
                  pingOnNewTicket,
                  pingOnTicketMessage,
                  isEnabled);
    }
    catch (Exception ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex,
                logMessage,
                ctx.User.Id,
                teamName,
                permissionLevel,
                pingOnNewTicket,
                pingOnTicketMessage,
                isEnabled);
    }
  }

  [SlashCommand("remove", "Remove a team.")]
  public async Task RemoveTeam(InteractionContext ctx,
                               [Autocomplete(typeof(TeamProvider))] [Option("teamName", "Team teamName")]
                               string teamName) {
    const string logMessage = $"[{nameof(TeamSlashCommands)}]{nameof(RemoveTeam)}({{ContextUserId}},{{TeamName}})";
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var team = await GuildTeam.GetByNameAsync(teamName);
      await team.ProcessRemoveTeamAsync();
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.TEAM_REMOVED_SUCCESSFULLY));
      Log.Information(logMessage, ctx.User.Id, teamName);
    }
    catch (BotExceptionBase ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex, logMessage, ctx.User.Id, teamName);
    }
    catch (Exception ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex, logMessage, ctx.User.Id, teamName);
    }
  }

  [SlashCommand("add-user", "Add a user to a team.")]
  public async Task AddTeamMember(InteractionContext ctx,
                                  [Autocomplete(typeof(TeamProvider))] [Option("teamName", "Team teamName")]
                                  string teamName,
                                  [Option("member", "Member to add to the team")]
                                  DiscordUser member) {
    const string logMessage = $"[{nameof(TeamSlashCommands)}]{nameof(AddTeamMember)}({{ContextUserId}},{{TeamName}},{{MemberId}})";
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    try {
      await DiscordUserInfo.AddOrUpdateAsync(member);

      var team = await GuildTeam.GetByNameAsync(teamName);
      await team.ProcessAddTeamMemberAsync(member.Id);
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.MEMBER_ADDED_TO_TEAM));
      Log.Information(logMessage, ctx.User.Id, teamName, member.Id);
    }
    catch (BotExceptionBase ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex, logMessage, ctx.User.Id, teamName, member.Id);
    }
    catch (Exception ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex, logMessage, ctx.User.Id, teamName, member.Id);
    }
  }

  [SlashCommand("remove-user", "Remove a user from a team.")]
  public async Task RemoveTeamMember(InteractionContext ctx,
                                     [Autocomplete(typeof(TeamProvider))] [Option("teamName", "Team teamName")]
                                     string teamName,
                                     [Option("member", "Member to remove from the team")]
                                     DiscordUser member) {
    const string logMessage = $"[{nameof(TeamSlashCommands)}]{nameof(RemoveTeamMember)}({{ContextUserId}},{{TeamName}},{{MemberId}})";
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var team = await GuildTeam.GetByNameAsync(teamName);
      await DiscordUserInfo.AddOrUpdateAsync(member);

      await team.ProcessRemoveTeamMember(member.Id);
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.MEMBER_REMOVED_FROM_TEAM));
      Log.Information(logMessage, ctx.User.Id, teamName, member.Id);
    }
    catch (BotExceptionBase ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex, logMessage, ctx.User.Id, teamName, member.Id);
    }
    catch (Exception ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex, logMessage, ctx.User.Id, teamName, member.Id);
    }
  }

  [SlashCommand("add-role", "Adds a role to a team.")]
  public async Task AddRoleToTeam(InteractionContext ctx,
                                  [Autocomplete(typeof(TeamProvider))] [Option("teamName", "Team teamName")]
                                  string teamName,
                                  [Option("role", "Role to add to the team")]
                                  DiscordRole role) {
    const string logMessage = $"[{nameof(TeamSlashCommands)}]{nameof(AddRoleToTeam)}({{ContextUserId}},{{TeamName}},{{RoleId}})";
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var team = await GuildTeam.GetByNameAsync(teamName);
      await team.ProcessAddRoleToTeam(role);
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.ROLE_ADDED_TO_TEAM));
      Log.Information(logMessage, ctx.User.Id, teamName, role.Id);
    }
    catch (BotExceptionBase ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex, logMessage, ctx.User.Id, teamName, role.Id);
    }
    catch (Exception ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex, logMessage, ctx.User.Id, teamName, role.Id);
    }
  }

  [SlashCommand("remove-role", "Removes a role from a team.")]
  public async Task RemoveRoleFromTeam(InteractionContext ctx,
                                       [Autocomplete(typeof(TeamProvider))] [Option("teamName", "Team teamName")]
                                       string teamName,
                                       [Option("role", "Role to remove from the team")]
                                       DiscordRole role) {
    const string logMessage = $"[{nameof(TeamSlashCommands)}]{nameof(RemoveRoleFromTeam)}({{ContextUserId}},{{TeamName}},{{RoleId}})";
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    try {
      var team = await GuildTeam.GetByNameAsync(teamName);
      await team.ProcessRemoveRoleFromTeam(role);
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.ROLE_REMOVED_FROM_TEAM));
      Log.Information(logMessage, ctx.User.Id, teamName, role.Id);
    }

    catch (BotExceptionBase ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex, logMessage, ctx.User.Id, teamName, role.Id);
    }
    catch (Exception ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex, logMessage, ctx.User.Id, teamName, role.Id);
    }
  }

  [SlashCommand("rename", "Rename a team.")]
  public async Task RenameTeam(InteractionContext ctx,
                               [Autocomplete(typeof(TeamProvider))] [Option("teamName", "Team teamName")]
                               string teamName,
                               [Option("newName", "New team name")] string newName) {
    const string logMessage = $"[{nameof(TeamSlashCommands)}]{nameof(RenameTeam)}({{ContextUserId}},{{TeamName}},{{NewName}})";
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var team = await GuildTeam.GetByNameAsync(teamName);
      await team.ProcessRenameAsync(newName);
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.TEAM_RENAMED_SUCCESSFULLY));
      Log.Information(logMessage, ctx.User.Id, teamName, newName);
    }
    catch (BotExceptionBase ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex, logMessage, ctx.User.Id, teamName, newName);
    }
    catch (Exception ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex, logMessage, ctx.User.Id, teamName, newName);
    }
  }
}
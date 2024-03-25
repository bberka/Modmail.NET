using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Attributes;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Providers;
using Modmail.NET.Static;

namespace Modmail.NET.Commands;

[SlashCommandGroup("team", "Team management commands.")]
[RequirePermissionLevelOrHigher(TeamPermissionLevel.Admin)]
[UpdateUserInformation]
[RequireMainServer]
public class TeamSlashCommands : ApplicationCommandModule
{
  [SlashCommand("list", "List all teams.")]
  public async Task ListTeams(InteractionContext ctx) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());

    var teams = await GuildTeam.GetAllAsync(ctx.Guild.Id);
    if (teams is null || teams.Count == 0) {
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Error(Texts.NO_TEAM_FOUND));
      return;
    }

    await ctx.Interaction.EditOriginalResponseAsync(CommandResponses.ListTeams(ctx.Guild, teams));
  }

  [SlashCommand("create", "Create a new team.")]
  public async Task CreateTeam(InteractionContext ctx,
                               [Option("teamName", "Team name")] string teamName,
                               [Option("permissionLevel", "Permission level")]
                               TeamPermissionLevel permissionLevel
  ) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var currentGuildId = ctx.Guild.Id;
    var guildOption = await GuildOption.GetAsync();
    if (guildOption is null) {
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Error(Texts.SERVER_NOT_SETUP));
      return;
    }

    var existingTeam = await GuildTeam.GetByNameAsync(guildOption.GuildId, teamName);
    if (existingTeam is not null) {
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Error(Texts.TEAM_WITH_SAME_NAME_ALREADY_EXISTS));
      return;
    }

    var team = new GuildTeam {
      GuildOptionId = currentGuildId,
      Name = teamName,
      RegisterDateUtc = DateTime.UtcNow,
      IsEnabled = true,
      GuildTeamMembers = new List<GuildTeamMember>(),
      UpdateDateUtc = null,
      Id = Guid.NewGuid(),
      PermissionLevel = permissionLevel
    };
    await team.AddAsync();
    await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.TEAM_CREATED_SUCCESSFULLY));
  }

  [SlashCommand("remove", "Remove a team.")]
  public async Task RemoveTeam(InteractionContext ctx,
                               [Autocomplete(typeof(TeamProvider))] [Option("teamName", "Team teamName")]
                               string teamName) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var team = await GuildTeam.GetByNameAsync(ctx.Guild.Id, teamName);
    if (team is null) {
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Error(Texts.TEAM_NOT_FOUND));
      return;
    }

    await team.RemoveAsync();
    await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.TEAM_REMOVED_SUCCESSFULLY));
  }

  [SlashCommand("add-user", "Add a user to a team.")]
  public async Task AddTeamMember(InteractionContext ctx,
                                  [Autocomplete(typeof(TeamProvider))] [Option("teamName", "Team teamName")]
                                  string teamName,
                                  [Option("member", "Member to add to the team")]
                                  DiscordUser member) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var team = await GuildTeam.GetByNameAsync(ctx.Guild.Id, teamName);

    if (team is null) {
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Error(Texts.TEAM_NOT_FOUND));
      return;
    }

    await DiscordUserInfo.AddOrUpdateAsync(member);


    var isUserAlreadyInTeam = await GuildTeamMember.IsUserInAnyTeamAsync(member.Id);
    if (isUserAlreadyInTeam) {
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.MEMBER_ALREADY_IN_TEAM));
      return;
    }

    var memberEntity = new GuildTeamMember {
      GuildTeamId = team.Id,
      Type = TeamMemberDataType.UserId,
      Key = member.Id,
      RegisterDateUtc = DateTime.UtcNow
    };
    team.GuildTeamMembers.Add(memberEntity);
    await team.UpdateAsync();

    await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.MEMBER_ADDED_TO_TEAM));
  }

  [SlashCommand("remove-user", "Remove a user from a team.")]
  public async Task RemoveTeamMember(InteractionContext ctx,
                                     [Autocomplete(typeof(TeamProvider))] [Option("teamName", "Team teamName")]
                                     string teamName,
                                     [Option("member", "Member to remove from the team")]
                                     DiscordUser member) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    var team = await GuildTeam.GetByNameAsync(ctx.Guild.Id, teamName);

    if (team is null) {
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Error(Texts.TEAM_NOT_FOUND));
      return;
    }

    await DiscordUserInfo.AddOrUpdateAsync(member);

    var memberEntity = team.GuildTeamMembers.FirstOrDefault(x => x.Key == member.Id);
    if (memberEntity is null) {
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Error(Texts.MEMBER_NOT_FOUND_IN_TEAM));
      return;
    }

    team.GuildTeamMembers.Remove(memberEntity);
    await team.UpdateAsync();

    await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.MEMBER_REMOVED_FROM_TEAM));
  }

  [SlashCommand("add-role", "Adds a role to a team.")]
  public async Task AddRoleToTeam(InteractionContext ctx,
                                  [Autocomplete(typeof(TeamProvider))] [Option("teamName", "Team teamName")]
                                  string teamName,
                                  [Option("role", "Role to add to the team")]
                                  DiscordRole role) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var team = await GuildTeam.GetByNameAsync(ctx.Guild.Id, teamName);

    if (team is null) {
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Error(Texts.TEAM_NOT_FOUND));
      return;
    }

    var isRoleAlreadyInTeam = await GuildTeamMember.IsRoleInAnyTeamAsync(role.Id);
    if (isRoleAlreadyInTeam) {
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Error(Texts.ROLE_ALREADY_IN_TEAM));
      return;
    }

    var roleEntity = new GuildTeamMember {
      GuildTeamId = team.Id,
      Type = TeamMemberDataType.RoleId,
      Key = role.Id,
      RegisterDateUtc = DateTime.UtcNow
    };
    team.GuildTeamMembers.Add(roleEntity);
    await team.UpdateAsync();

    await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.ROLE_ADDED_TO_TEAM));
  }

  [SlashCommand("remove-role", "Removes a role from a team.")]
  public async Task RemoveRoleFromTeam(InteractionContext ctx,
                                       [Autocomplete(typeof(TeamProvider))] [Option("teamName", "Team teamName")]
                                       string teamName,
                                       [Option("role", "Role to remove from the team")]
                                       DiscordRole role) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var team = await GuildTeam.GetByNameAsync(ctx.Guild.Id, teamName);

    if (team is null) {
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Error(Texts.TEAM_NOT_FOUND));
      return;
    }

    var roleEntity = team.GuildTeamMembers.FirstOrDefault(x => x.Key == role.Id);
    if (roleEntity is null) {
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Error(Texts.ROLE_NOT_FOUND_IN_TEAM));
      return;
    }

    team.GuildTeamMembers.Remove(roleEntity);
    await team.UpdateAsync();

    await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.ROLE_REMOVED_FROM_TEAM));
  }

  [SlashCommand("rename", "Rename a team.")]
  public async Task RenameTeam(InteractionContext ctx,
                               [Autocomplete(typeof(TeamProvider))] [Option("teamName", "Team teamName")]
                               string teamName,
                               [Option("newName", "New team name")] string newName) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var team = await GuildTeam.GetByNameAsync(ctx.Guild.Id, teamName);

    if (team is null) {
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Error(Texts.TEAM_NOT_FOUND));
      return;
    }

    team.Name = newName;
    await team.UpdateAsync();

    await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.TEAM_RENAMED_SUCCESSFULLY));
  }
}
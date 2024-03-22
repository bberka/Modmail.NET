using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Attributes;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Providers;
using Modmail.NET.Static;

namespace Modmail.NET.Commands;

[SlashCommandGroup("team", "Team management commands.")]
[RequirePermissionLevelOrHigher(TeamPermissionLevel.Admin)]
public class TeamSlashCommands : ApplicationCommandModule
{
  [SlashCommand("list", "List all teams.")]
  public async Task ListTeams(InteractionContext ctx) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());

    var currentGuildId = ctx.Guild.Id;
    if (currentGuildId != MMConfig.This.MainServerId) {
      var embed3 = ModmailEmbeds.Base(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var dbService = ServiceLocator.Get<IDbService>();


    var teams = await dbService.GetTeamsAsync(currentGuildId);
    if (teams is null || teams.Count == 0) {
      var embed2 = ModmailEmbeds.Base("No teams found!", "", DiscordColor.Red);

      var builder = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var embed = ModmailEmbeds.ListTeams(ctx.Guild, teams);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }

  [SlashCommand("create", "Create a new team.")]
  public async Task CreateTeam(InteractionContext ctx,
                               [Option("teamName", "Team name")] string teamName,
                               [Option("permissionLevel", "Permission level")]
                               TeamPermissionLevel permissionLevel
  ) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var currentGuildId = ctx.Guild.Id;
    if (currentGuildId != MMConfig.This.MainServerId) {
      var embed3 = ModmailEmbeds.Base(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var dbService = ServiceLocator.Get<IDbService>();

    var guildOption = await dbService.GetOptionAsync(currentGuildId);
    if (guildOption is null) {
      var embed2 = ModmailEmbeds.Base("Server not setup!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var existingTeam = (await dbService.GetTeamsAsync(currentGuildId)).FirstOrDefault(x => x.Name == teamName);
    if (existingTeam is not null) {
      var embed2 = ModmailEmbeds.Base("Team already exists!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
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
    await dbService.AddTeamAsync(team);

    var embed = ModmailEmbeds.Base("Team created!", "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }

  [SlashCommand("remove", "Remove a team.")]
  public async Task RemoveTeam(InteractionContext ctx,
                               [Autocomplete(typeof(TeamProvider))] [Option("teamName", "Team teamName")]
                               string teamName) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var currentGuildId = ctx.Guild.Id;
    if (currentGuildId != MMConfig.This.MainServerId) {
      var embed3 = ModmailEmbeds.Base(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var dbService = ServiceLocator.Get<IDbService>();

    var team = await dbService.GetTeamByNameAsync(currentGuildId, teamName);
    if (team is null) {
      var embed2 = ModmailEmbeds.Base("Team not found!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    await dbService.RemoveTeamAsync(team);

    var embed = ModmailEmbeds.Base("Team removed!", "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }

  [SlashCommand("add-user", "Add a user to a team.")]
  public async Task AddTeamMember(InteractionContext ctx,
                                  [Autocomplete(typeof(TeamProvider))] [Option("teamName", "Team teamName")]
                                  string teamName,
                                  [Option("member", "Member to add to the team")]
                                  DiscordUser member) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var currentGuildId = ctx.Guild.Id;
    if (currentGuildId != MMConfig.This.MainServerId) {
      var embed3 = ModmailEmbeds.Base(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var dbService = ServiceLocator.Get<IDbService>();

    var team = await dbService.GetTeamByNameAsync(currentGuildId, teamName);

    if (team is null) {
      var embed2 = ModmailEmbeds.Base("Team not found!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    await dbService.UpdateUserInfoAsync(new DiscordUserInfo(member));


    var isUserAlreadyInTeam = await dbService.IsUserInAnyTeamAsync(member.Id);
    if (isUserAlreadyInTeam) {
      var embed2 = ModmailEmbeds.Base("Member already in a team!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var memberEntity = new GuildTeamMember {
      GuildTeamId = team.Id,
      Type = TeamMemberDataType.UserId,
      Key = member.Id,
      RegisterDateUtc = DateTime.UtcNow
    };
    team.GuildTeamMembers.Add(memberEntity);
    await dbService.UpdateTeamAsync(team);

    var embed = ModmailEmbeds.Base("Member added to team!", "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }

  [SlashCommand("remove-user", "Remove a user from a team.")]
  public async Task RemoveTeamMember(InteractionContext ctx,
                                     [Autocomplete(typeof(TeamProvider))] [Option("teamName", "Team teamName")]
                                     string teamName,
                                     [Option("member", "Member to remove from the team")]
                                     DiscordUser member) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var currentGuildId = ctx.Guild.Id;
    if (currentGuildId != MMConfig.This.MainServerId) {
      var embed3 = ModmailEmbeds.Base(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var dbService = ServiceLocator.Get<IDbService>();


    var team = await dbService.GetTeamByNameAsync(currentGuildId, teamName);

    if (team is null) {
      var embed2 = ModmailEmbeds.Base("Team not found!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    await dbService.UpdateUserInfoAsync(new DiscordUserInfo(member));

    var memberEntity = team.GuildTeamMembers.FirstOrDefault(x => x.Key == member.Id);
    if (memberEntity is null) {
      var embed2 = ModmailEmbeds.Base("Member not found in the team!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    team.GuildTeamMembers.Remove(memberEntity);
    await dbService.UpdateTeamAsync(team);

    var embed = ModmailEmbeds.Base("Member removed from team!", "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }

  [SlashCommand("add-role", "Adds a role to a team.")]
  public async Task AddRoleToTeam(InteractionContext ctx,
                                  [Autocomplete(typeof(TeamProvider))] [Option("teamName", "Team teamName")]
                                  string teamName,
                                  [Option("role", "Role to add to the team")]
                                  DiscordRole role) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var currentGuildId = ctx.Guild.Id;
    if (currentGuildId != MMConfig.This.MainServerId) {
      var embed3 = ModmailEmbeds.Base(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var dbService = ServiceLocator.Get<IDbService>();

    var team = await dbService.GetTeamByNameAsync(currentGuildId, teamName);

    if (team is null) {
      var embed2 = ModmailEmbeds.Base("Team not found!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var isRoleAlreadyInTeam = await dbService.IsRoleInAnyTeamAsync(role.Id);
    if (isRoleAlreadyInTeam) {
      var embed2 = ModmailEmbeds.Base("Role already in a team!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var roleEntity = new GuildTeamMember {
      GuildTeamId = team.Id,
      Type = TeamMemberDataType.RoleId,
      Key = role.Id,
      RegisterDateUtc = DateTime.UtcNow
    };
    team.GuildTeamMembers.Add(roleEntity);
    await dbService.UpdateTeamAsync(team);

    var embed = ModmailEmbeds.Base("Role added to team!", "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }

  [SlashCommand("remove-role", "Removes a role from a team.")]
  public async Task RemoveRoleFromTeam(InteractionContext ctx,
                                       [Autocomplete(typeof(TeamProvider))] [Option("teamName", "Team teamName")]
                                       string teamName,
                                       [Option("role", "Role to remove from the team")]
                                       DiscordRole role) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var currentGuildId = ctx.Guild.Id;
    if (currentGuildId != MMConfig.This.MainServerId) {
      var embed3 = ModmailEmbeds.Base(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }


    var dbService = ServiceLocator.Get<IDbService>();

    var team = await dbService.GetTeamByNameAsync(currentGuildId, teamName);

    if (team is null) {
      var embed2 = ModmailEmbeds.Base("Team not found!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var roleEntity = team.GuildTeamMembers.FirstOrDefault(x => x.Key == role.Id);
    if (roleEntity is null) {
      var embed2 = ModmailEmbeds.Base("Role not found in the team!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    team.GuildTeamMembers.Remove(roleEntity);
    await dbService.UpdateTeamAsync(team);

    var embed = ModmailEmbeds.Base("Role removed from team!", "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }

  [SlashCommand("rename", "Rename a team.")]
  public async Task RenameTeam(InteractionContext ctx,
                               [Autocomplete(typeof(TeamProvider))] [Option("teamName", "Team teamName")]
                               string teamName,
                               [Option("newName", "New team name")] string newName) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var currentGuildId = ctx.Guild.Id;
    if (currentGuildId != MMConfig.This.MainServerId) {
      var embed3 = ModmailEmbeds.Base(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER, "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed3);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    var dbService = ServiceLocator.Get<IDbService>();

    var team = await dbService.GetTeamByNameAsync(currentGuildId, teamName);

    if (team is null) {
      var embed2 = ModmailEmbeds.Base("Team not found!", "", DiscordColor.Red);
      var builder = new DiscordWebhookBuilder().AddEmbed(embed2);
      await ctx.Interaction.EditOriginalResponseAsync(builder);
      return;
    }

    team.Name = newName;
    await dbService.UpdateTeamAsync(team);

    var embed = ModmailEmbeds.Base("Team renamed!", "", DiscordColor.Green);
    var builder2 = new DiscordWebhookBuilder().AddEmbed(embed);
    await ctx.Interaction.EditOriginalResponseAsync(builder2);
  }
}
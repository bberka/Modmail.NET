using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Features.Guild.Commands;
using Modmail.NET.Features.Guild.Queries;
using Modmail.NET.Features.Guild.Static;
using Modmail.NET.Features.Permission.Queries;
using Modmail.NET.Features.Permission.Static;
using Modmail.NET.Features.Teams.Static;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Guild.Handlers;

public class ProcessCreateLogChannelHandlers : IRequestHandler<ProcessCreateLogChannelCommand, DiscordChannel>
{
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessCreateLogChannelHandlers(ISender sender, ModmailDbContext dbContext) {
    _sender = sender;
    _dbContext = dbContext;
  }

  public async Task<DiscordChannel> Handle(ProcessCreateLogChannelCommand request, CancellationToken cancellationToken) {
    var guild = await _sender.Send(new GetDiscordMainGuildQuery(), cancellationToken);
    var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken) ?? throw new NullReferenceException();

    var permissions = await _sender.Send(new GetPermissionInfoOrHigherQuery(TeamPermissionLevel.Admin), cancellationToken);
    var members = await guild.GetAllMembersAsync(cancellationToken).ToListAsync(cancellationToken);
    var roles = guild.Roles;

    var roleListForOverwrites = new List<DiscordRole>();
    var memberListForOverwrites = new List<DiscordMember>();
    foreach (var perm in permissions) {
      var role = roles.FirstOrDefault(x => x.Key == perm.Key && perm.Type == TeamMemberDataType.RoleId);
      if (role.Key != 0) roleListForOverwrites.Add(role.Value);
      var member2 = members.FirstOrDefault(x => x.Id == perm.Key && perm.Type == TeamMemberDataType.UserId);
      if (member2 is not null && member2.Id != 0) memberListForOverwrites.Add(member2);
    }

    var permissionOverwrites = UtilPermission.GetTicketPermissionOverwrites(guild, memberListForOverwrites, roleListForOverwrites);

    DiscordChannel category;
    try {
      category = await guild.GetChannelAsync(guildOption.CategoryId);
    }
    catch (NotFoundException) {
      category = await guild.CreateChannelCategoryAsync(GuildConstants.CategoryName, permissionOverwrites);
    }

    var logChannel = await guild.CreateTextChannelAsync(GuildConstants.LogChannelName, category, LangProvider.This.GetTranslation(LangKeys.ModmailLogChannelTopic), permissionOverwrites);
    guildOption.LogChannelId = logChannel.Id;
    guildOption.CategoryId = category.Id;

    _dbContext.Update(guildOption);

    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    return logChannel;
  }
}
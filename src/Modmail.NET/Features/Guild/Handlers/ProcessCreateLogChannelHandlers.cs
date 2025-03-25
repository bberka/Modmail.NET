using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Bot;
using Modmail.NET.Features.Permission;
using Modmail.NET.Utils;

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
      category = await guild.CreateChannelCategoryAsync(Const.CategoryName, permissionOverwrites);
    }

    var logChannel = await guild.CreateTextChannelAsync(Const.LogChannelName, category, LangProvider.This.GetTranslation(LangKeys.MODMAIL_LOG_CHANNEL_TOPIC), permissionOverwrites);
    guildOption.LogChannelId = logChannel.Id;
    guildOption.CategoryId = category.Id;

    _dbContext.Update(guildOption);

    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    return logChannel;
  }
}
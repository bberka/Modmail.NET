using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Features.Server.Commands;
using Modmail.NET.Features.Server.Queries;
using Modmail.NET.Features.Server.Static;
using Modmail.NET.Features.Teams.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Server.Handlers;

public class ProcessCreateOrUpdateLogChannelHandlers : IRequestHandler<ProcessCreateOrUpdateLogChannelCommand, DiscordChannel>
{
    private readonly ModmailDbContext _dbContext;
    private readonly ISender _sender;

    public ProcessCreateOrUpdateLogChannelHandlers(ISender sender, ModmailDbContext dbContext)
    {
        _sender = sender;
        _dbContext = dbContext;
    }

    public async ValueTask<DiscordChannel> Handle(ProcessCreateOrUpdateLogChannelCommand request, CancellationToken cancellationToken)
    {
        var guild = await _sender.Send(new GetDiscordMainServerQuery(), cancellationToken);
        var option = await _sender.Send(new GetOptionQuery(), cancellationToken);


        DiscordChannel? logChannel = null;
        var createChannel = option.LogChannelId == 0;
        if (!createChannel)
            try
            {
                logChannel = await guild.GetChannelAsync(option.LogChannelId);
            }
            catch (NotFoundException)
            {
                createChannel = true;
            }

        if (createChannel)
        {
            var permissionOverwrites = await GetLogChannelPermissionOverwrites(guild, cancellationToken);
            DiscordChannel category;
            try
            {
                category = await guild.GetChannelAsync(option.CategoryId);
            }
            catch (NotFoundException)
            {
                category = await guild.CreateChannelCategoryAsync(ServerConstants.CategoryName, permissionOverwrites);
            }

            logChannel = await guild.CreateTextChannelAsync(ServerConstants.LogChannelName, category, Lang.ModmailLogChannelTopic.Translate(),
                permissionOverwrites);
            option.LogChannelId = logChannel.Id;
            option.CategoryId = category.Id;

            _dbContext.Update(option);

            var affected = await _dbContext.SaveChangesAsync(cancellationToken);
            if (affected == 0) throw new DbInternalException();

            return logChannel;
        }
        else
        {
            //TODO: Create a BG job that checks team updates and syncs the permissions to channel every hour or so also for all ticket channels
            var permissionOverwrites = await GetLogChannelPermissionOverwrites(guild, cancellationToken);
            if (logChannel is not null) await logChannel.ModifyAsync(x => { x.PermissionOverwrites = permissionOverwrites; });


            DiscordChannel? category = null;
            try
            {
                category = await guild.GetChannelAsync(option.CategoryId);
            }
            catch (NotFoundException)
            {
                //category deosnt exists but channel does exists, which means channel is moved we can ignore updating category permissions
            }

            if (category is not null) await category.ModifyAsync(x => { x.PermissionOverwrites = permissionOverwrites; });
        }

        return logChannel ?? throw new NullReferenceException(nameof(logChannel));
    }

    private async Task<DiscordOverwriteBuilder[]> GetLogChannelPermissionOverwrites(DiscordGuild guild, CancellationToken cancellationToken)
    {
        var users = await _sender.Send(new GetUserTeamInformationQuery(), cancellationToken);
        var members = await guild.GetAllMembersAsync(cancellationToken)
            .ToListAsync(cancellationToken);

        var memberListForOverwrites = users.Select(user => members.FirstOrDefault(x => x.Id == user.UserId))
            .Where(member => member is not null && member.Id != 0)
            .Select(x => x!)
            .ToArray();

        var permissionOverwrites = UtilPermission.GetTicketPermissionOverwrites(guild, memberListForOverwrites);
        return permissionOverwrites;
    }
}
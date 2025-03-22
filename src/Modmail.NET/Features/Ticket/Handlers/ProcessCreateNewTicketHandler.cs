using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;
using Modmail.NET.Features.Teams;
using Modmail.NET.Features.TicketType;
using Modmail.NET.Jobs;
using Modmail.NET.Utils;

namespace Modmail.NET.Features.Ticket.Handlers;

public sealed class ProcessCreateNewTicketHandler : IRequestHandler<ProcessCreateNewTicketCommand>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;
  private readonly TicketTypeSelectionTimeoutJob _ticketTypeSelectionTimeoutJob;

  public ProcessCreateNewTicketHandler(ISender sender,
                                       ModmailBot bot,
                                       ModmailDbContext dbContext,
                                       TicketTypeSelectionTimeoutJob ticketTypeSelectionTimeoutJob) {
    _sender = sender;
    _bot = bot;
    _dbContext = dbContext;
    _ticketTypeSelectionTimeoutJob = ticketTypeSelectionTimeoutJob;
  }

  public async Task Handle(ProcessCreateNewTicketCommand request, CancellationToken cancellationToken) {
    var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken);

    var guild = await _bot.GetMainGuildAsync();
    //make new privateChannel
    var channelName = string.Format(Const.TicketNameTemplate, request.User.Username.Trim());
    var category = await _bot.Client.GetChannelAsync(guildOption.CategoryId);

    var ticketId = Guid.NewGuid();

    var permissions = await _sender.Send(new GetTeamPermissionInfoQuery(), cancellationToken);
    var members = await guild.GetAllMembersAsync(cancellationToken).ToListAsync(cancellationToken: cancellationToken);
    var roles = guild.Roles;

    var (modMemberListForOverwrites, modRoleListForOverwrites) = UtilPermission.ParsePermissionInfo(permissions, members, roles);
    var permissionOverwrites = UtilPermission.GetTicketPermissionOverwrites(guild, modMemberListForOverwrites, modRoleListForOverwrites);
    var mailChannel = await guild.CreateTextChannelAsync(channelName, category, UtilChannelTopic.BuildChannelTopic(ticketId), permissionOverwrites);

    var member = await _bot.GetMemberFromAnyGuildAsync(request.User.Id);
    if (member is null) return;


    var newTicketMessageBuilder = TicketResponses.NewTicket(member, ticketId, permissions);


    var ticketMessage = TicketMessage.MapFrom(ticketId, request.Message);

    var ticket = new Entities.Ticket {
      OpenerUserId = request.User.Id,
      ModMessageChannelId = mailChannel.Id,
      RegisterDateUtc = DateTime.UtcNow,
      PrivateMessageChannelId = request.PrivateChannel.Id,
      InitialMessageId = request.Message.Id,
      Priority = TicketPriority.Normal,
      LastMessageDateUtc = DateTime.UtcNow,
      Id = ticketId,
      Anonymous = guildOption.AlwaysAnonymous,
      IsForcedClosed = false,
      CloseReason = null,
      FeedbackMessage = null,
      FeedbackStar = null,
      CloserUserId = null,
      ClosedDateUtc = null,
      TicketTypeId = null,
      Messages = [ticketMessage],
      BotTicketCreatedMessageInDmId = 0
    };


    var ticketTypes = await _sender.Send(new GetTicketTypeListQuery(true), cancellationToken);

    var ticketCreatedMessage = await request.PrivateChannel.SendMessageAsync(UserResponses.YouHaveCreatedNewTicket(guild,
                                                                                                                   guildOption,
                                                                                                                   ticketTypes,
                                                                                                                   ticketId));

    _ticketTypeSelectionTimeoutJob.AddMessage(ticketCreatedMessage);


    var dmTicketCreatedMessage = await request.PrivateChannel.SendMessageAsync(UserResponses.MessageSent(request.Message));

    ticket.BotTicketCreatedMessageInDmId = dmTicketCreatedMessage.Id;

    _dbContext.Tickets.Add(ticket);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();


    _ = Task.Run(async () => {
      await mailChannel.SendMessageAsync(newTicketMessageBuilder);
      await mailChannel.SendMessageAsync(TicketResponses.MessageReceived(request.Message));


      if (guildOption.IsEnableDiscordChannelLogging) {
        var newTicketCreatedLog = LogResponses.NewTicketCreated(request.Message, mailChannel, ticket.Id);
        var logChannel = await _bot.GetLogChannelAsync();
        await logChannel.SendMessageAsync(newTicketCreatedLog);
        if (guildOption.IsSensitiveLogging) await logChannel.SendMessageAsync(LogResponses.MessageSentByUser(request.Message, ticket.Id));
      }
    }, cancellationToken);
  }
}
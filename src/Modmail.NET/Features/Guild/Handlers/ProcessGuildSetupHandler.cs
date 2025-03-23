using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;

namespace Modmail.NET.Features.Guild.Handlers;

public sealed class ProcessGuildSetupHandler : IRequestHandler<ProcessGuildSetupCommand, GuildOption>
{
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessGuildSetupHandler(ModmailDbContext dbContext, ISender sender) {
    _dbContext = dbContext;
    _sender = sender;
  }

  public async Task<GuildOption> Handle(ProcessGuildSetupCommand request, CancellationToken cancellationToken) {
    var existingMmOption = await _sender.Send(new GetGuildOptionQuery(true), cancellationToken);
    if (existingMmOption is not null) throw new MainServerAlreadySetupException();

    var anyServerSetup = await _sender.Send(new AnyGuildSetupQuery(), cancellationToken);
    if (anyServerSetup) throw new AnotherServerAlreadySetupException();

    var guildOption = new GuildOption {
      CategoryId = 0,
      LogChannelId = 0,
      GuildId = request.Guild.Id,
      IsEnabled = true,
      RegisterDateUtc = DateTime.UtcNow,
      TakeFeedbackAfterClosing = false,
      ShowConfirmationWhenClosingTickets = false,
      IconUrl = request.Guild.IconUrl,
      Name = request.Guild.Name,
      BannerUrl = request.Guild.BannerUrl,
      TicketTimeoutHours = Const.DefaultTicketTimeoutHours
    };

    await _sender.Send(new ClearGuildOptionCommand(), cancellationToken);
    _dbContext.Add(guildOption);
    await _dbContext.SaveChangesAsync(cancellationToken);
    return guildOption;
  }
}
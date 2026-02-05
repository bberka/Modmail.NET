using MediatR;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Guild.Commands;
using Modmail.NET.Features.Guild.Queries;

namespace Modmail.NET.Features.Guild.Handlers;

public class ProcessGuildSetupHandler : IRequestHandler<ProcessGuildSetupCommand, GuildOption>
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

    var anyServerSetup = await _sender.Send(new CheckAnyGuildSetupQuery(), cancellationToken);
    if (anyServerSetup) throw new AnotherServerAlreadySetupException();

    var guildOption = new GuildOption {
      CategoryId = 0,
      LogChannelId = 0,
      GuildId = request.Guild.Id,
      IsEnabled = true,
      RegisterDateUtc = UtilDate.GetNow(),
      TakeFeedbackAfterClosing = false,
      IconUrl = request.Guild.IconUrl,
      Name = request.Guild.Name,
      BannerUrl = request.Guild.BannerUrl
    };

    await _sender.Send(new ClearGuildOptionCommand(request.AuthorizedUserId), cancellationToken);
    _dbContext.Add(guildOption);
    await _dbContext.SaveChangesAsync(cancellationToken);
    return guildOption;
  }
}
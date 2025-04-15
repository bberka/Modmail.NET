using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Features.Server.Commands;
using Modmail.NET.Features.Server.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Server.Handlers;

public class ProcessSetupHandler : IRequestHandler<ProcessSetupCommand, Option>
{
	private readonly ModmailDbContext _dbContext;
	private readonly ISender _sender;

	public ProcessSetupHandler(ModmailDbContext dbContext, ISender sender) {
		_dbContext = dbContext;
		_sender = sender;
	}

	public async ValueTask<Option> Handle(ProcessSetupCommand request, CancellationToken cancellationToken) {
		var existingMmOption = await _dbContext.Options.FirstOrDefaultAsync(cancellationToken);
		if (existingMmOption is not null) throw new ModmailBotException(Lang.MainServerAlreadySetup);

		var anyServerSetup = await _sender.Send(new CheckSetupQuery(), cancellationToken);
		if (anyServerSetup) throw new ModmailBotException(Lang.AnotherServerAlreadySetup);

		await _sender.Send(new ClearOptionCommand(request.AuthorizedUserId), cancellationToken);

		var guildOption = new Option {
			CategoryId = 0,
			LogChannelId = 0,
			ServerId = request.Guild.Id,
			IsEnabled = true,
			RegisterDateUtc = UtilDate.GetNow(),
			TakeFeedbackAfterClosing = false,
			IconUrl = request.Guild.IconUrl,
			Name = request.Guild.Name,
			BannerUrl = request.Guild.BannerUrl
		};

		_dbContext.Add(guildOption);
		var affected = await _dbContext.SaveChangesAsync(cancellationToken);
		if (affected == 0) throw new DbInternalException();

		_ = await _sender.Send(new GetDiscordLogChannelQuery(), cancellationToken);
		return guildOption;
	}
}
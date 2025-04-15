using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Features.Ticket.Queries;

namespace Modmail.NET.Features.Ticket.Handlers;

public class CheckTicketTypeExistsHandler : IRequestHandler<CheckTicketTypeExistsQuery, bool>
{
	private readonly ModmailDbContext _dbContext;

	public CheckTicketTypeExistsHandler(ModmailDbContext dbContext) {
		_dbContext = dbContext;
	}

	public async ValueTask<bool> Handle(CheckTicketTypeExistsQuery request, CancellationToken cancellationToken) {
		return await _dbContext.TicketTypes.AnyAsync(x => x.Name == request.NameOrKey || x.Key == request.NameOrKey, cancellationToken);
	}
}
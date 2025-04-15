using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Modmail.NET.Common.Static;
using Modmail.NET.Database;
using Modmail.NET.Features.Teams.Queries;

namespace Modmail.NET.Features.Teams.Handlers;

public class GetUserPermissionsHandler : IRequestHandler<GetUserPermissionsQuery, AuthPolicy[]>
{
	private readonly IDbContextFactory<ModmailDbContext> _factory;
	private readonly IOptions<BotConfig> _options;

	public GetUserPermissionsHandler(IDbContextFactory<ModmailDbContext> factory,
	                                 IOptions<BotConfig> options) {
		_factory = factory;
		_options = options;
	}

	public async ValueTask<AuthPolicy[]> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken) {
		if (_options.Value.SuperUsers.Contains(request.UserId)) return [AuthPolicy.SuperUser];

		var dbContext = await _factory.CreateDbContextAsync(cancellationToken);
		var permissions = await dbContext.TeamPermissions
		                                 .AsNoTracking()
		                                 .Include(x => x.Team)
		                                 .ThenInclude(x => x!.Users)
		                                 .Where(x => x.Team!.Users.Any(y => y.UserId == request.UserId))
		                                 .Select(x => x.AuthPolicy)
		                                 .ToArrayAsync(cancellationToken);

		if (permissions.Contains(AuthPolicy.SuperUser)) return [AuthPolicy.SuperUser]; //TODO: implement this logic also to update permission logic if allow all provided then 

		return permissions;
	}
}
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Features.Teams.Queries;

namespace Modmail.NET.Features.Teams.Handlers;

public class CheckUserInAnyTeamHandler : IRequestHandler<CheckUserInAnyTeamQuery, bool>
{
    private readonly ModmailDbContext _dbContext;

    public CheckUserInAnyTeamHandler(ModmailDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<bool> Handle(CheckUserInAnyTeamQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.TeamUsers.AnyAsync(x => x.UserId == request.UserId, cancellationToken);
    }
}
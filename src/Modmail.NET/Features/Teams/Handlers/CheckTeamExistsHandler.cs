using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Features.Teams.Queries;

namespace Modmail.NET.Features.Teams.Handlers;

public class CheckTeamExistsHandler : IRequestHandler<CheckTeamExistsQuery, bool>
{
    private readonly ModmailDbContext _dbContext;

    public CheckTeamExistsHandler(ModmailDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<bool> Handle(CheckTeamExistsQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Teams.AnyAsync(x => x.Name == request.Name, cancellationToken);
    }
}
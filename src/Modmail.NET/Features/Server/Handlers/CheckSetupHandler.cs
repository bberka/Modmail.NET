using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Features.Server.Queries;

namespace Modmail.NET.Features.Server.Handlers;

public class CheckSetupHandler : IRequestHandler<CheckSetupQuery, bool>
{
    private readonly ModmailDbContext _dbContext;

    public CheckSetupHandler(ModmailDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<bool> Handle(CheckSetupQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Options.AnyAsync(cancellationToken);
    }
}
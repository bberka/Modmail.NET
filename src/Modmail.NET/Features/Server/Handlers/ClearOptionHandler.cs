using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Features.Server.Commands;

namespace Modmail.NET.Features.Server.Handlers;

public class ClearOptionHandler : IRequestHandler<ClearOptionCommand>
{
    private readonly ModmailDbContext _dbContext;

    public ClearOptionHandler(ModmailDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Unit> Handle(ClearOptionCommand request, CancellationToken cancellationToken)
    {
        var options = await _dbContext.Options.ToListAsync(cancellationToken);
        _dbContext.Options.RemoveRange(options);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
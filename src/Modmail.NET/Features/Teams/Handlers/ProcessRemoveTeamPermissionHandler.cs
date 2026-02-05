using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Teams.Commands;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Teams.Handlers;

public class ProcessRemoveTeamPermissionHandler : IRequestHandler<ProcessRemoveTeamPermissionCommand, TeamPermission>
{
    private readonly ModmailDbContext _dbContext;

    public ProcessRemoveTeamPermissionHandler(ModmailDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<TeamPermission> Handle(ProcessRemoveTeamPermissionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.TeamPermissions.Include(x => x.Team)
            .Where(x => x.AuthPolicy == request.AuthPolicy)
            .FirstOrDefaultAsync(cancellationToken);
        if (entity is null) throw new ModmailBotException(Lang.PermissionNotFound);
        if (entity.Team!.SuperUserTeam) throw new InvalidOperationException();

        _dbContext.Remove(entity);
        var affected = await _dbContext.SaveChangesAsync(cancellationToken);
        if (affected == 0) throw new DbInternalException();

        return entity;
    }
}
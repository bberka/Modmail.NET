using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Teams.Commands;
using Serilog;

namespace Modmail.NET.Features.Teams.Handlers;

public class SyncSuperUserTeamUsersHandler : IRequestHandler<SyncSuperUserTeamUsersCommand>
{
    private readonly ModmailDbContext _dbContext;
    private readonly IOptions<BotConfig> _options;

    public SyncSuperUserTeamUsersHandler(ModmailDbContext dbContext, IOptions<BotConfig> options)
    {
        _dbContext = dbContext;
        _options = options;
    }

    public async ValueTask<Unit> Handle(SyncSuperUserTeamUsersCommand request, CancellationToken cancellationToken)
    {
        var team = await GetOrCreateSuperUserTeam(cancellationToken);
        SyncTeamPermissions(team);
        await SyncTeamMembers(team, cancellationToken);
        return Unit.Value;
    }

    private async Task<Team> GetOrCreateSuperUserTeam(CancellationToken cancellationToken)
    {
        var team = await _dbContext.Teams.Where(x => x.SuperUserTeam)
            .Include(x => x.Permissions)
            .Include(x => x.Users)
            .AsSplitQuery()
            .SingleOrDefaultAsync(cancellationToken);

        if (team is not null) return team;

        var teamId = Guid.NewGuid();
        team = new Team
        {
            Id = teamId,
            Name = "ConfigSuperUsers",
            SuperUserTeam = true,
            Permissions =
            [
                new TeamPermission
                {
                    AuthPolicy = AuthPolicy.SuperUser,
                    TeamId = teamId
                }
            ]
        };
        _dbContext.Add(team);

        Log.Information("[{Source}] Created SuperUserTeam. TeamId: {TeamId}", nameof(SyncSuperUserTeamUsersHandler), team.Id);
        return team;
    }

    private static void SyncTeamPermissions(Team team)
    {
        if (team.Permissions.Count != 1 || team.Permissions.All(x => x.AuthPolicy != AuthPolicy.SuperUser))
        {
            team.Permissions.Clear();
            team.Permissions.Add(new TeamPermission
            {
                TeamId = team.Id,
                AuthPolicy = AuthPolicy.SuperUser
            });
            Log.Warning("[{Source}] SuperUserTeam had invalid permissions, corrected. TeamId: {TeamId}", nameof(SyncSuperUserTeamUsersHandler),
                team.Id);
        }
    }

    private async Task SyncTeamMembers(Team team, CancellationToken cancellationToken)
    {
        var configSuperUsers = _options.Value.SuperUsers;

        // Check for users in config that aren't on user list, in any team
        var usersToAdd = configSuperUsers.Except(team.Users.Select(x => x.UserId))
            .ToList();
        if (usersToAdd.Count != 0)
        {
            await EnsureUsersAreOnlyInThisTeam(team, usersToAdd, cancellationToken);

            usersToAdd.ForEach(userId => team.Users.Add(new TeamUser
            {
                TeamId = team.Id,
                UserId = userId
            }));
            Log.Information("[{Source}] Added missing super users to SuperUserTeam. TeamId: {TeamId}, UserIds: {UserIds}",
                nameof(SyncSuperUserTeamUsersHandler), team.Id, string.Join(", ", usersToAdd));
        }

        // Remove extra super users if they aren't in config
        var usersToRemove = team.Users.Select(x => x.UserId)
            .Except(configSuperUsers)
            .ToList();
        if (usersToRemove.Count != 0)
        {
            var teamUsersToRemove = team.Users.Where(x => usersToRemove.Contains(x.UserId))
                .ToList();
            _dbContext.RemoveRange(teamUsersToRemove);
            Log.Information("[{Source}] Removed extra super users from SuperUserTeam. TeamId: {TeamId}, UserIds: {UserIds}",
                nameof(SyncSuperUserTeamUsersHandler), team.Id, string.Join(", ", usersToRemove));
        }

        var affected = await _dbContext.SaveChangesAsync(cancellationToken);
        if (affected > 0)
            Log.Information("[{Source}] Updated SuperUserTeam. TeamId: {TeamId}, Changes: {Changes}", nameof(SyncSuperUserTeamUsersHandler), team.Id,
                affected);
        else
            Log.Information("[{Source}] SuperUserTeam is synced. TeamId: {TeamId}", nameof(SyncSuperUserTeamUsersHandler), team.Id);
    }

    private async Task EnsureUsersAreOnlyInThisTeam(
        Team targetTeam,
        List<ulong> userIds,
        CancellationToken cancellationToken
    )
    {
        // Check if the accounts from config have access or are used to some team.

        var teamsWithTheseUsers = await _dbContext.Teams.Include(x => x.Users)
            .Where(x => x.Users.Any(y => userIds.Contains(y.UserId)) && x.Id != targetTeam.Id)
            .ToListAsync(cancellationToken);

        if (teamsWithTheseUsers.Any())
            // remove users from other teams
            foreach (var t in teamsWithTheseUsers)
            {
                var usersToRemove = t.Users.Where(x => userIds.Contains(x.UserId))
                    .ToList();
                _dbContext.RemoveRange(usersToRemove);
                Log.Information("[{Source}] Removed super users from other teams. TeamId: {TeamId}, UserIds: {UserIds}",
                    nameof(SyncSuperUserTeamUsersHandler), t.Id, string.Join(", ", usersToRemove.Select(x => x.UserId)));
            }

        // Now save these changes and remove this
        var affectedRemove = await _dbContext.SaveChangesAsync(cancellationToken);
        if (affectedRemove == 0) throw new DbInternalException();
    }
}
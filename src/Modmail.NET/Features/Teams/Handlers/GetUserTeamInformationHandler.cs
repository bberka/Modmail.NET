using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Features.Teams.Models;
using Modmail.NET.Features.Teams.Queries;

namespace Modmail.NET.Features.Teams.Handlers;

public class GetUserTeamInformationHandler : IRequestHandler<GetUserTeamInformationQuery, UserTeamInformation[]>
{
  private readonly ModmailDbContext _dbContext;

  public GetUserTeamInformationHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<UserTeamInformation[]> Handle(GetUserTeamInformationQuery request, CancellationToken cancellationToken) {
    var members = await _dbContext.Teams
                                  .Include(x => x.Users)
                                  .ThenInclude(x => x.Team)
                                  .SelectMany(x => x.Users.Select(y => new UserTeamInformation(y.UserId, x.PingOnNewTicket, x.PingOnNewMessage)))
                                  .ToArrayAsync(cancellationToken);

    return members;
  }
}
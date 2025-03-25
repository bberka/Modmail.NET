using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;

namespace Modmail.NET.Features.Statistic.Handlers;

public class GetLatestStatisticHandler : IRequestHandler<GetLatestStatisticQuery, Entities.Statistic>
{
  private readonly ModmailDbContext _dbContext;

  public GetLatestStatisticHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<Entities.Statistic> Handle(GetLatestStatisticQuery request, CancellationToken cancellationToken) {
    var data = await _dbContext.Statistics.OrderByDescending(x => x.RegisterDateUtc).FirstOrDefaultAsync(cancellationToken);
    if (!request.AllowNull && data is null) throw new NotFoundException(LangKeys.Statistic);

    return data;
  }
}
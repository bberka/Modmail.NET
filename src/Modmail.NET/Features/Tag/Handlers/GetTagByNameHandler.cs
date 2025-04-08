using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Features.Tag.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Tag.Handlers;

public class GetTagByNameHandler : IRequestHandler<GetTagByNameQuery, Database.Entities.Tag>
{
  private readonly ModmailDbContext _dbContext;

  public GetTagByNameHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<Database.Entities.Tag> Handle(GetTagByNameQuery request, CancellationToken cancellationToken) {
    var tag = await _dbContext.Tags.Where(x => x.Name == request.Name).FirstOrDefaultAsync(cancellationToken);
    if (tag is null) throw new ModmailBotException(LangKeys.TagDoesntExists);

    return tag;
  }
}
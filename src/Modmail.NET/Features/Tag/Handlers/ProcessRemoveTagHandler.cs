using MediatR;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Features.Tag.Commands;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Tag.Handlers;

public class ProcessRemoveTagHandler : IRequestHandler<ProcessRemoveTagCommand, Database.Entities.Tag>
{
  private readonly ModmailDbContext _dbContext;

  public ProcessRemoveTagHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<Database.Entities.Tag> Handle(ProcessRemoveTagCommand request, CancellationToken cancellationToken) {
    var entity = await _dbContext.Tags.FindAsync([request.Id], cancellationToken);

    if (entity is null) throw new NotFoundException(LangKeys.Tag);

    _dbContext.Remove(entity);

    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    return entity;
  }
}
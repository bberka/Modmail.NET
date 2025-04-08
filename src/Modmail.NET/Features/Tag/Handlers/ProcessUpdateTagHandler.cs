using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Features.Tag.Commands;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Tag.Handlers;

public class ProcessUpdateTagHandler : IRequestHandler<ProcessUpdateTagCommand, Database.Entities.Tag>
{
  private readonly ModmailDbContext _dbContext;

  public ProcessUpdateTagHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<Database.Entities.Tag> Handle(ProcessUpdateTagCommand request, CancellationToken cancellationToken) {
    var fixedName = request.Name.Trim().Replace(" ", "-").ToLower();

    var entity = await _dbContext.Tags.Where(x => x.Name == fixedName).FirstOrDefaultAsync(cancellationToken: cancellationToken);
    if (entity is null) {
      throw new ModmailBotException(LangKeys.TagDoesntExists);
    }

    var isNameSame = fixedName.Equals(entity.Name, StringComparison.InvariantCultureIgnoreCase);
    if (!isNameSame) {
      var exists = await _dbContext.Tags.Where(x => x.Name == fixedName).AnyAsync(cancellationToken: cancellationToken);
      if (exists) {
        throw new ModmailBotException(LangKeys.TagWithSameNameAlreadyExists);
      }
    }

    entity.Title = request.Title.Trim();
    entity.Content = request.Content.Trim();
    entity.Name = fixedName;

    _dbContext.Update(entity);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    return entity;
  }
}
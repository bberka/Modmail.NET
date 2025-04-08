using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Features.Tag.Commands;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Tag.Handlers;

public class ProcessCreateTagHandler : IRequestHandler<ProcessCreateTagCommand, Database.Entities.Tag>
{
  private readonly ModmailDbContext _dbContext;

  public ProcessCreateTagHandler(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<Database.Entities.Tag> Handle(ProcessCreateTagCommand request, CancellationToken cancellationToken) {
    var fixedName = request.Name.Trim().Replace(" ", "-").ToLower();

    var exists = await _dbContext.Tags.Where(x => x.Name == fixedName).AnyAsync(cancellationToken: cancellationToken);
    if (exists) {
      throw new ModmailBotException(LangKeys.TagWithSameNameAlreadyExists);
    }

    var entity = new Database.Entities.Tag {
      Name = fixedName,
      Title = request.Title.Trim(),
      Content = request.Content.Trim(),
    };
    _dbContext.Add(entity);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    return entity;
  }
}
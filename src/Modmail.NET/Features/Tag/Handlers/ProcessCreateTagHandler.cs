using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.Tag.Commands;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Tag.Handlers;

public class ProcessCreateTagHandler : IRequestHandler<ProcessCreateTagCommand>
{
    private readonly ModmailDbContext _dbContext;

    public ProcessCreateTagHandler(ModmailDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Unit> Handle(ProcessCreateTagCommand request, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Tags.FilterByTagName(request.Name)
            .AnyAsync(cancellationToken);
        if (exists) throw new ModmailBotException(Lang.TagWithSameNameAlreadyExists);

        var embedId = Guid.CreateVersion7();
        var tag = new Database.Entities.Tag
        {
            Name = request.Name,
            EmbedId = embedId,
            Embed = request.Embed,
            IncludeAuthorWhenTicketChannel = request.IncludeAuthorWhenTicketChannel
        };

        _dbContext.Add(tag);
        var affected = await _dbContext.SaveChangesAsync(cancellationToken);
        if (affected == 0) throw new DbInternalException();

        return Unit.Value;
    }
}
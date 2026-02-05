using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.Tag.Commands;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Tag.Handlers;

public class ProcessUpdateTagHandler : IRequestHandler<ProcessUpdateTagCommand, Database.Entities.Tag>
{
    private readonly ModmailDbContext _dbContext;

    public ProcessUpdateTagHandler(ModmailDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Database.Entities.Tag> Handle(ProcessUpdateTagCommand request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Tags.Include(x => x.Embed)
            .Include(x => x.Embed.Fields)
            .FilterById(request.TagId)
            .FirstOrDefaultAsync(cancellationToken);
        if (entity is null) throw new ModmailBotException(Lang.TagDoesntExists);

        var isNameSame = request.Name.Equals(entity.Name, StringComparison.InvariantCultureIgnoreCase);
        if (!isNameSame)
        {
            var exists = await _dbContext.Tags.FilterByTagName(request.Name)
                .AnyAsync(cancellationToken);
            if (exists) throw new ModmailBotException(Lang.TagWithSameNameAlreadyExists);
        }

        entity.Embed.AuthorId = request.Embed.AuthorId;
        entity.Embed.Color = request.Embed.Color;
        entity.Embed.Description = request.Embed.Description;
        entity.Embed.ImageUrl = request.Embed.ImageUrl;
        entity.Embed.ThumbnailUrl = request.Embed.ThumbnailUrl;
        entity.Embed.Title = request.Embed.Title;
        entity.Embed.WithServerInfoFooter = request.Embed.WithServerInfoFooter;
        entity.Embed.WithTimestamp = request.Embed.WithTimestamp;
        entity.Embed.IncludeAuthor = request.IncludeAuthorWhenTicketChannel;
        entity.Embed.Fields.Clear();
        foreach (var field in request.Embed.Fields) entity.Embed.Fields.Add(field);

        _dbContext.Update(entity);
        var affected = await _dbContext.SaveChangesAsync(cancellationToken);
        if (affected == 0) throw new DbInternalException();

        return entity;
    }
}
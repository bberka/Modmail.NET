using System.Reflection;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;
using Modmail.NET.Features.Teams.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Pipeline;

public class PermissionCheckPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IPermissionCheck, IRequest<TResponse>
{
    private readonly ISender _sender;

    public PermissionCheckPipelineBehavior(ISender sender)
    {
        _sender = sender;
    }

    public async ValueTask<TResponse> Handle(
        TRequest message,
        CancellationToken cancellationToken,
        MessageHandlerDelegate<TRequest, TResponse> next
    )
    {
        if (message.AuthorizedUserId <= 0) throw new ModmailBotException(Lang.UnauthorizedAccess);
        var attribute = message.GetType()
            .GetCustomAttribute<RequireModmailPermissionAttribute>();
        if (attribute is null) throw new InvalidOperationException();
        if (attribute.AuthPolicy is null)
        {
            // This means that the command is not a permission check but team user check
            var isAnyTeamMember = await _sender.Send(new CheckUserInAnyTeamQuery(message.AuthorizedUserId), cancellationToken);
            if (!isAnyTeamMember) throw new ModmailBotException(Lang.UnauthorizedAccess);
            return await next(message, cancellationToken);
        }

        var allowed = await _sender.Send(new CheckPermissionAccessQuery(message.AuthorizedUserId, attribute.AuthPolicy), cancellationToken);
        if (!allowed) throw new ModmailBotException(Lang.UnauthorizedAccess);

        return await next(message, cancellationToken);
    }
}
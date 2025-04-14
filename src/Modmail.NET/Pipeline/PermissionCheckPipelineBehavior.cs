using System.Reflection;
using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;
using Modmail.NET.Features.Teams.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Pipeline;

public class PermissionCheckPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  where TRequest : IPermissionCheck, IRequest<TResponse>
{
  private readonly ISender _sender;

  public PermissionCheckPipelineBehavior(ISender sender) {
    _sender = sender;
  }

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
    var attribute = typeof(TRequest).GetCustomAttribute<RequireModmailPermissionAttribute>();
    if (attribute == null) throw new InvalidOperationException();

    if (attribute.AuthPolicy is null) {
      var isAnyTeamMember = await _sender.Send(new CheckUserInAnyTeamQuery(request.AuthorizedUserId), cancellationToken);
      if (!isAnyTeamMember) throw new ModmailBotException(Lang.UnauthorizedAccess);
      return await next(cancellationToken);
    }

    var allowed = await _sender.Send(new CheckPermissionAccessQuery(request.AuthorizedUserId, attribute.AuthPolicy), cancellationToken);
    if (!allowed) throw new ModmailBotException(Lang.UnauthorizedAccess);

    return await next(cancellationToken);
  }
}
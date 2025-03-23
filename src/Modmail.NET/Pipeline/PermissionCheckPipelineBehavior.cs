using System.Reflection;
using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;
using Modmail.NET.Features.Teams;

namespace Modmail.NET.Pipeline;

public sealed class PermissionCheckPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  where TRequest : IPermissionCheck, IRequest<TResponse>
{
  private readonly ModmailBot _bot;
  private readonly ISender _sender;

  public PermissionCheckPipelineBehavior(ModmailBot bot, ISender sender) {
    _bot = bot;
    _sender = sender;
  }

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
    var cacheAttribute = typeof(TRequest).GetCustomAttribute<PermissionCheckAttribute>();
    if (cacheAttribute == null) return await next();


    if (request.AuthorizedUserId <= 0) {
      throw new InvalidUserIdException();
    }

    if (_bot.Client.CurrentUser.Id == request.AuthorizedUserId) {
      return await next();
    }

    var guild = await _bot.GetMainGuildAsync();
    var member = await guild.GetMemberAsync(request.AuthorizedUserId);
    var roleList = member.Roles.Select(x => x.Id).ToArray();
    var permission = await _sender.Send(new GetTeamPermissionLevelQuery(request.AuthorizedUserId, roleList), cancellationToken);
    if (permission is null) {
      throw new UnauthorizedAccessException();
    }

    var option = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken);
    if (permission < option.ManageBlacklistMinAccessLevel) {
      throw new UnauthorizedAccessException();
    }

    return await next();
  }
}
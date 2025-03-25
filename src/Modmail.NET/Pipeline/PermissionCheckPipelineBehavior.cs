using System.Reflection;
using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;
using Modmail.NET.Features.Permission;
using Modmail.NET.Features.Teams;

namespace Modmail.NET.Pipeline;

public class PermissionCheckPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  where TRequest : IPermissionCheck, IRequest<TResponse>
{
  private readonly ISender _sender;

  public PermissionCheckPipelineBehavior(ISender sender) {
    _sender = sender;
  }

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
    var attribute = typeof(TRequest).GetCustomAttribute<PermissionCheckAttribute>();
    if (attribute == null) return await next();

    var allowed = await _sender.Send(new CheckPermissionAccessQuery(request.AuthorizedUserId, AuthPolicy.FromName(attribute.Policy)), cancellationToken);
    if (!allowed) throw new UnauthorizedAccessException();

    return await next();
  }
}
using MediatR;
using Microsoft.Extensions.Options;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Static;
using Modmail.NET.Features.Teams.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Teams.Handlers;

public class CheckPermissionAccessHandler : IRequestHandler<CheckPermissionAccessQuery, bool>
{
  private readonly ModmailBot _bot;
  private readonly IOptions<BotConfig> _options;
  private readonly ISender _sender;

  public CheckPermissionAccessHandler(ModmailBot bot,
                                      ISender sender,
                                      IOptions<BotConfig> options) {
    _bot = bot;
    _sender = sender;
    _options = options;
  }


  public async Task<bool> Handle(CheckPermissionAccessQuery request, CancellationToken cancellationToken) {
    if (request.UserId <= 0) throw new ModmailBotException(Lang.InvalidUser);

    if (_bot.Client.CurrentUser.Id == request.UserId) return true;

    var isConfigOwner = _options.Value.SuperUsers.Contains(request.UserId);
    if (isConfigOwner) return true;

    var permissions = await _sender.Send(new GetUserPermissionsQuery(request.UserId), cancellationToken);
    return permissions.Any(x => x.Name == request.Policy.Name || x.Name == AuthPolicy.SuperUser.Name);
  }
}
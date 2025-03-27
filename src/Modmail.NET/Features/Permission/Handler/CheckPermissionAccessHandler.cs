using MediatR;
using Microsoft.Extensions.Options;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;

namespace Modmail.NET.Features.Permission.Handler;

public class CheckPermissionAccessHandler : IRequestHandler<CheckPermissionAccessQuery, bool>
{
  private static readonly IReadOnlyDictionary<AuthPolicy, TeamPermissionLevel> PolicyMap = new Dictionary<AuthPolicy, TeamPermissionLevel> {
    { AuthPolicy.Support, TeamPermissionLevel.Support },
    { AuthPolicy.Moderator, TeamPermissionLevel.Moderator },
    { AuthPolicy.Admin, TeamPermissionLevel.Admin },
    { AuthPolicy.Owner, TeamPermissionLevel.Owner }
  };

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
    if (request.UserId <= 0) throw new InvalidUserIdException();

    if (_bot.Client.CurrentUser.Id == request.UserId) return true;

    var isConfigOwner = _options.Value.OwnerUsers.Contains(request.UserId);
    if (isConfigOwner) return true;

    var userPermissionLevel = await _sender.Send(new GetPermissionLevelQuery(request.UserId), cancellationToken);
    if (userPermissionLevel is null) return false;

    var option = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken);


    if (PolicyMap.TryGetValue(request.Policy, out var requiredLevel)) {
      if (userPermissionLevel >= requiredLevel) return true;
    }
    else if (request.Policy == AuthPolicy.ManageTickets) {
      if (userPermissionLevel >= option.ManageTicketMinAccessLevel) return true;
    }
    else if (request.Policy == AuthPolicy.ManageTicketTypes) {
      if (userPermissionLevel >= option.ManageTicketTypeMinAccessLevel) return true;
    }
    else if (request.Policy == AuthPolicy.ManageTeams) {
      if (userPermissionLevel >= option.ManageTeamsMinAccessLevel) return true;
    }
    else if (request.Policy == AuthPolicy.ManageBlacklist) {
      if (userPermissionLevel >= option.ManageBlacklistMinAccessLevel) return true;
    }
    else if (request.Policy == AuthPolicy.ManageHangfire) {
      if (userPermissionLevel >= option.ManageHangfireMinAccessLevel) return true;
    }

    return false;
  }
}
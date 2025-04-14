using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Modmail.NET.Web.Blazor.Extensions;

namespace Modmail.NET.Web.Blazor.Providers;

public class DiscordAuthenticationStateProvider : AuthenticationStateProvider
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public DiscordAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor) {
    _httpContextAccessor = httpContextAccessor;
  }

  public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
    var user = _httpContextAccessor.HttpContext?.User;
    if (user == null || user.Identity?.IsAuthenticated != true) return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    var userId = user.GetUserId(); //validates team user authenticate
    return new AuthenticationState(user);
  }
}
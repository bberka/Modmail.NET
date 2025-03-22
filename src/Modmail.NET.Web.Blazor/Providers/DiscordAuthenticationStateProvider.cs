using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;


namespace Modmail.NET.Web.Blazor.Providers;

public class DiscordAuthenticationStateProvider : AuthenticationStateProvider
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public DiscordAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor) {
    _httpContextAccessor = httpContextAccessor;
  }

  public override Task<AuthenticationState> GetAuthenticationStateAsync() {
    var user = _httpContextAccessor.HttpContext?.User;

    if (user == null || user.Identity?.IsAuthenticated != true) {
      return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
    }

    return Task.FromResult(new AuthenticationState(user));
  }
}
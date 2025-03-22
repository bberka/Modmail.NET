using System.Security.Claims;
using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Modmail.NET.Web.Blazor.Controllers;

[Route("auth")]
public class AuthController : Controller
{
  [HttpGet("login")]
  public IActionResult Login() {
    return Challenge(new AuthenticationProperties {
      RedirectUri = "/auth/callback",
      IsPersistent = true
    }, DiscordAuthenticationDefaults.AuthenticationScheme);
  }

  [HttpGet("logout")]
  public async Task<IActionResult> Logout() {
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Redirect("/");
  }
}
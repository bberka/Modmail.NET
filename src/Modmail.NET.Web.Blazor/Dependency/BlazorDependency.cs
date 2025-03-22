using Modmail.NET.Static;
using Radzen;

namespace Modmail.NET.Web.Blazor.Dependency;

public static class BlazorDependency
{
  public static void Configure(WebApplicationBuilder builder) {
    builder.Services.AddRadzenCookieThemeService(options => {
      options.Name = Const.ThemeCookieName; // The name of the cookie
      options.Duration = TimeSpan.FromDays(365); // The duration of the cookie
    });

    builder.Services.AddRadzenComponents();

    builder.Services.AddRazorComponents()
           .AddInteractiveServerComponents();
  }
}
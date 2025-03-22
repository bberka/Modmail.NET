using System.Security.Claims;
using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Teams;
using Modmail.NET.Web.Blazor.Providers;
using Modmail.NET.Web.Blazor.Static;
using Serilog;

namespace Modmail.NET.Web.Blazor.Dependency;

public static class AuthDependency
{
  public static void Configure(WebApplicationBuilder builder) {
    builder.Services.AddAuthentication(options => {
             options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
             options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
             options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
           })
           .AddCookie(x => {
             x.LoginPath = "/auth/login";
             x.LogoutPath = "/auth/logout";
             x.AccessDeniedPath = "/forbidden";
             x.ExpireTimeSpan = TimeSpan.FromDays(1);
             x.SlidingExpiration = true;
             x.Cookie.HttpOnly = true; // Prevent client-side access
             x.Cookie.SecurePolicy = CookieSecurePolicy.Always;
             x.Cookie.SameSite = SameSiteMode.Lax;
           })
           .AddDiscord(options => {
             options.ClientId = builder.Configuration.GetValue<string>("Bot:BotClientId") ?? throw new Exception("BotClientId is empty");
             options.ClientSecret = builder.Configuration.GetValue<string>("Bot:BotClientSecret") ?? throw new Exception("BotClientSecret is empty");
             options.CallbackPath = "/auth/callback"; // Redirect URI
             options.SaveTokens = true; // Save tokens for later use
             options.Scope.Add("identify"); // Fetch user details
             options.Scope.Add("guilds"); // Fetch guilds (optional, for roles)
             options.AccessDeniedPath = "/result?m=AccessDenied";
             options.CorrelationCookie.SameSite = SameSiteMode.Lax;
             options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
             options.Events.OnCreatingTicket += async context => {
               if (context.Principal?.Identity?.IsAuthenticated != true) {
                 context.Fail("Invalid Token");
                 return;
               }

               var identity = (ClaimsIdentity)context.Principal.Identity;
               identity.AddClaim(new Claim("access_token", context.AccessToken ?? string.Empty));
               identity.AddClaim(new Claim("refresh_token", context.RefreshToken ?? string.Empty));
               identity.AddClaim(new Claim("token_type", context.TokenType ?? string.Empty));

               var identifier = context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
               if (identifier is null) {
                 context.Fail("Invalid Token");
                 return;
               }

               var userId = ulong.Parse(identifier);

               var scope = context.HttpContext.RequestServices.CreateScope();
               var sender = scope.ServiceProvider.GetRequiredService<ISender>();

               var bot = scope.ServiceProvider.GetRequiredService<ModmailBot>();
               try {
                 var guild = await bot.GetMainGuildAsync();
                 var discordMember = await guild.GetMemberAsync(userId);
                 var roles = discordMember.Roles.Select(x => x.Id).ToList();
                 var permission = await sender.Send(new GetTeamPermissionLevelQuery(userId, roles));
                 if (permission is null) {
                   context.Fail("Not member of any team");
                   return;
                 }

                 identity.AddClaim(new Claim(ClaimTypes.Role, permission.ToString() ?? throw new NullReferenceException()));
                 Log.Information("Discord.OAuth access granted {UserId} {UserName} {Permission}", userId, discordMember.DisplayName, permission.ToString());
                 context.Success();
               }
               catch (BotExceptionBase ex) {
                 context.Fail(ex.TitleMessage + " : " + ex.ContentMessage);
               }
               catch (Exception ex) {
                 context.Fail(ex);
               }
             };
             options.Events.OnRemoteFailure += context => {
               context.Response.Redirect("/result?m=RemoteAuthFailure");
               return Task.CompletedTask;
             };
             options.Events.OnAccessDenied += context => {
               context.Response.Redirect("/result?m=AccessDenied");
               return Task.CompletedTask;
             };
           });


    var authorizationBuilder = builder.Services.AddAuthorizationBuilder();

    foreach (var policy in AuthPolicy.List) {
      authorizationBuilder.AddPolicy(policy.Name, p => p.Requirements.Add(new TeamPermissionCheckRequirement(policy)));
    }


    builder.Services.AddSingleton<IAuthorizationHandler, TeamPermissionCheckHandler>();

    builder.Services.AddScoped<AuthenticationStateProvider, DiscordAuthenticationStateProvider>();
  }
}
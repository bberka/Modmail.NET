using System.Security.Claims;
using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Static;
using Modmail.NET.Common.Utils;
using Modmail.NET.Features.Teams.Queries;
using Modmail.NET.Language;
using Modmail.NET.Web.Blazor.Providers;
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
			       x.AccessDeniedPath = "/result/" + Lang.ErrorAccessDenied;
			       x.ExpireTimeSpan = TimeSpan.FromDays(1);
			       x.SlidingExpiration = true;
			       x.Cookie.HttpOnly = true;
			       x.Cookie.SecurePolicy = CookieSecurePolicy.Always;
			       x.Cookie.Name = "auth_cookie";
			       x.Cookie.SameSite = SameSiteMode.Lax;
			       x.Events.OnSigningIn = async context => {
				       if (context.Principal == null || context.Principal.Identity?.IsAuthenticated != true) await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			       };
		       })
		       .AddDiscord(x => {
			       x.ClientId = builder.Configuration.GetValue<string>("Bot:BotClientId") ?? throw new Exception("BotClientId is empty");
			       x.ClientSecret = builder.Configuration.GetValue<string>("Bot:BotClientSecret") ?? throw new Exception("BotClientSecret is empty");
			       x.CallbackPath = "/auth/callback"; // Redirect URI
			       x.SaveTokens = true; // Save tokens for later use
			       x.Scope.Add("identify"); // Fetch user details
			       x.Scope.Add("guilds"); // Fetch guilds (optional, for roles)
			       x.AccessDeniedPath = "/result/" + Lang.ErrorAccessDenied;
			       x.CorrelationCookie.SameSite = SameSiteMode.Lax;
			       x.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
			       x.Events.OnCreatingTicket += async context => {
				       context.Properties.RedirectUri = "/dashboard";
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
					       Log.Warning("Discord.OAuth failed, UserIdentity is null");
					       context.Fail("Invalid Token");
					       return;
				       }

				       var userId = ulong.Parse(identifier);
				       var name = context.Principal.FindFirst(ClaimTypes.Name)?.Value;
				       var scope = context.HttpContext.RequestServices.CreateScope();
				       var sender = scope.ServiceProvider.GetRequiredService<ISender>();
				       var options = scope.ServiceProvider.GetRequiredService<IOptions<BotConfig>>();
				       try {
					       var isUserOwnerInSettings = options.Value.SuperUsers.Contains(userId);
					       var inAnyTeam = await sender.Send(new CheckUserInAnyTeamQuery(userId));
					       if (!isUserOwnerInSettings && !inAnyTeam) {
						       Log.Warning("Discord.OAuth failed, User is not in any team {UserId} {UserName}", userId, name);
						       context.Fail("Invalid Token");
						       return;
					       }

					       var permissions = await sender.Send(new GetUserPermissionsQuery(userId));
					       if (permissions.Length == 0) {
						       Log.Warning("Discord.OAuth Role permission check failed, user does not have permission {UserId} {UserName}", userId, name);
						       context.Fail("Not member of any team");
						       return;
					       }

					       identity.AddClaim(new Claim(AuthConstants.PermissionsClaimType, UtilPermission.ParseToPermissionsString(permissions)));
					       identity.AddClaim(new Claim(ClaimTypes.Role, AuthConstants.AuthorizeTeamRole));
					       Log.Information("Discord.OAuth access granted {UserId} {UserName}", userId, name);
					       context.Success();
				       }
				       catch (ModmailBotException ex) {
					       Log.Warning(ex, "Discord.OAuth Access failed {UserId} {UserName}", userId, name);
					       context.Fail(ex.TitleMessage + " : " + ex.ContentMessage);
				       }
				       catch (Exception ex) {
					       Log.Warning(ex, "Discord.OAuth Access exception occurred {UserId} {UserName}", userId, name);
					       context.Fail(ex);
				       }
			       };
			       x.Events.OnRemoteFailure += context => {
				       context.Response.Redirect("/result/" + Lang.ErrorAuthRemoteFail);
				       return Task.CompletedTask;
			       };
			       x.Events.OnAccessDenied += context => {
				       context.Response.Redirect("/result/" + Lang.ErrorAccessDenied);
				       return Task.CompletedTask;
			       };
		       });


		var authorizationBuilder = builder.Services.AddAuthorizationBuilder();

		foreach (var policy in AuthPolicy.List) authorizationBuilder.AddPolicy(policy.Name, p => p.Requirements.Add(new TeamPermissionCheckRequirement(policy)));


		builder.Services.AddSingleton<IAuthorizationHandler, TeamPermissionCheckHandler>();

		builder.Services.AddScoped<AuthenticationStateProvider, DiscordAuthenticationStateProvider>();
	}
}
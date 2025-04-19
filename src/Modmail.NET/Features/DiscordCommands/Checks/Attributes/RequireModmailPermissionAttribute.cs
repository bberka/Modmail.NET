using DSharpPlus.Commands.ContextChecks;

namespace Modmail.NET.Features.DiscordCommands.Checks.Attributes;

public class RequireModmailPermissionAttribute : ContextCheckAttribute
{
	public RequireModmailPermissionAttribute(string authPolicy) {
		AuthPolicy = AuthPolicy.FromName(authPolicy);
	}

	public RequireModmailPermissionAttribute() { }
	public AuthPolicy? AuthPolicy { get; }
}
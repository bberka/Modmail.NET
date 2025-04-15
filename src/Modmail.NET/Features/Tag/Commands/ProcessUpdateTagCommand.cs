using Modmail.NET.Abstract;
using Modmail.NET.Common.Static;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Tag.Commands;

[RequireModmailPermission(nameof(AuthPolicy.ManageTags))]
public sealed record ProcessUpdateTagCommand(ulong AuthorizedUserId, Guid Id, string Name, string? Title, string Content) : IRequest<Database.Entities.Tag>,
                                                                                                                            IPermissionCheck;
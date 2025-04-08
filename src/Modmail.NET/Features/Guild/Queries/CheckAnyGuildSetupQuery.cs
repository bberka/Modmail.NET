using MediatR;

namespace Modmail.NET.Features.Guild.Queries;

public sealed record CheckAnyGuildSetupQuery : IRequest<bool>;
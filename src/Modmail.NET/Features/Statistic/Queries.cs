using MediatR;
using Modmail.NET.Attributes;

namespace Modmail.NET.Features.Statistic;

[CachePolicy(nameof(GetLatestStatisticQuery), 60)]
public sealed record GetLatestStatisticQuery(bool AllowNull) : IRequest<Entities.Statistic>;
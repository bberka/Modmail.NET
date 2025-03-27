using MediatR;
using Modmail.NET.Attributes;
using Modmail.NET.Models.Dto;

namespace Modmail.NET.Features.Metric;

[CachePolicy(nameof(GetLatestMetricQuery), 60 * 60,false)]
public sealed record GetLatestMetricQuery(bool AllowNull) : IRequest<MetricDto>;
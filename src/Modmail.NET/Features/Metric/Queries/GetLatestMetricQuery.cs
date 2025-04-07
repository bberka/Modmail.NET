using MediatR;
using Modmail.NET.Attributes;
using Modmail.NET.Features.Metric.Models;

namespace Modmail.NET.Features.Metric.Queries;

[CachePolicy(nameof(GetLatestMetricQuery), 60 * 60, false)]
public sealed record GetLatestMetricQuery(bool AllowNull) : IRequest<MetricDto>;
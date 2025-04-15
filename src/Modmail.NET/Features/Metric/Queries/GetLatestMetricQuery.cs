using Modmail.NET.Attributes;
using Modmail.NET.Features.Metric.Models;

namespace Modmail.NET.Features.Metric.Queries;

[CachePolicy(nameof(GetLatestMetricQuery), 3600, false)]
public sealed record GetLatestMetricQuery : IRequest<MetricDto?>;
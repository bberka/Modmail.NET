using MediatR;
using Modmail.NET.Attributes;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.Server.Queries;

[CachePolicy("GetOptionQuery", 60)]
public sealed record GetOptionQuery : IRequest<Option>;
using MediatR;

namespace Modmail.NET.Features.Tag.Queries;

public sealed record GetTagByNameQuery(string Name) : IRequest<Database.Entities.Tag>;
using Modmail.NET.Features.Ticket.Models;
using Riok.Mapperly.Abstractions;

namespace Modmail.NET.Features.Ticket.Mappers;

[Mapper]
public static partial class TicketDtoMapper
{
    public static partial IQueryable<TicketDto> ProjectToDto(this IQueryable<Database.Entities.Ticket> queryable);

    public static partial TicketDto ToDto(this Database.Entities.Ticket entity);
}
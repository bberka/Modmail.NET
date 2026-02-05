using Modmail.NET.Features.Ticket.Models;
using Riok.Mapperly.Abstractions;

namespace Modmail.NET.Features.Ticket.Mappers;

[Mapper]
public static partial class TicketFeedbackDtoMapper
{
    public static partial IQueryable<TicketFeedbackDto> ProjectToFeedbackDto(this IQueryable<Database.Entities.Ticket> queryable);

    public static partial TicketFeedbackDto ToFeedbackDto(this Database.Entities.Ticket entity);
}
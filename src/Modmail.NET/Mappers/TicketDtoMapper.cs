using Modmail.NET.Entities;
using Modmail.NET.Models.Dto;
using Riok.Mapperly.Abstractions;

namespace Modmail.NET.Mappers;

[Mapper]
public static partial class TicketDtoMapper
{
  public static partial IQueryable<TicketDto> ProjectToDto(this IQueryable<Ticket> queryable);
  
  public static partial TicketDto ToDto(this Ticket entity);
}
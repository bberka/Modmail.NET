using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DSharpPlus.Entities;
using Modmail.NET.Database;
using Modmail.NET.Utils;

namespace Modmail.NET.Entities;

public class TicketMessage
{
  [Key]
  public Guid Id { get; set; }

  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;
  public ulong DiscordUserInfoId { get; set; }
  public string MessageContent { get; set; }
  public ulong MessageDiscordId { get; set; }


  [ForeignKey(nameof(Ticket))]
  public Guid TicketId { get; set; }

  //FK
  public virtual DiscordUserInfo DiscordUserInfo { get; set; }
  public virtual Ticket Ticket { get; set; }
  public virtual List<TicketMessageAttachment> TicketMessageAttachments { get; set; }

  public async Task AddAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    await dbContext.TicketMessages.AddAsync(this);
    await dbContext.SaveChangesAsync();
  }

  public async Task UpdateAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    dbContext.TicketMessages.Update(this);
    await dbContext.SaveChangesAsync();
  }

  public static TicketMessage MapFrom(Guid ticketId, DiscordMessage message) {
    var id = Guid.NewGuid();
    return new TicketMessage {
      Id = id,
      DiscordUserInfoId = message.Author.Id,
      MessageContent = message.Content,
      TicketId = ticketId,
      TicketMessageAttachments = message.Attachments.Select(x => TicketMessageAttachment.MapFrom(x, id)).ToList(),
      MessageDiscordId = message.Id,
      RegisterDateUtc = DateTime.UtcNow,
    };
  }

  public static TicketMessage MapFrom(Guid ticketId, ulong authorId, ulong messageId, string messageContent, List<DiscordAttachment>? discordAttachments) {
    discordAttachments ??= new List<DiscordAttachment>();
    var id = Guid.NewGuid();
    return new TicketMessage {
      Id = id,
      DiscordUserInfoId = authorId,
      MessageContent = messageContent,
      TicketId = ticketId,
      TicketMessageAttachments = discordAttachments.Select(x => TicketMessageAttachment.MapFrom(x, id)).ToList(),
      MessageDiscordId = messageId,
      RegisterDateUtc = UtilDate.GetNow(),
    };
  }
}
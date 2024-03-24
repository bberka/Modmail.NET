using System.ComponentModel.DataAnnotations;
using Modmail.NET.Common;
using Modmail.NET.Database;

namespace Modmail.NET.Entities;

public class TicketMessage
{
  [Key]
  public Guid Id { get; set; }

  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;
  public ulong DiscordUserInfoId { get; set; }
  public string MessageContent { get; set; }
  public ulong MessageDiscordId { get; set; }

  //FK

  public virtual DiscordUserInfo DiscordUserInfo { get; set; }
  public virtual Guid TicketId { get; set; }
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
}
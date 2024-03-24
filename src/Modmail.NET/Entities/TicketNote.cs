using System.ComponentModel.DataAnnotations;
using Modmail.NET.Common;
using Modmail.NET.Database;

namespace Modmail.NET.Entities;

public class TicketNote
{
  [Key]
  public Guid Id { get; set; }

  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;
  public string Content { get; set; }
  public Guid TicketId { get; set; }
  public ulong DiscordUserInfoId { get; set; }
  public string Username { get; set; }
  public virtual Ticket Ticket { get; set; }


  public virtual DiscordUserInfo DiscordUserInfo { get; set; }

  public async Task AddAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    await dbContext.TicketNotes.AddAsync(this);
    await dbContext.SaveChangesAsync();
  }
}
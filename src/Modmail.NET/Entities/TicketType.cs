using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;

namespace Modmail.NET.Entities;

public class TicketType
{
  [Key]
  public Guid Id { get; set; }

  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;
  public DateTime? UpdateDateUtc { get; set; }
  public string Key { get; set; }
  public string Name { get; set; }
  public string? Emoji { get; set; }
  public string? Description { get; set; }
  public int Order { get; set; }
  public string EmbedMessageTitle { get; set; }
  public string EmbedMessageContent { get; set; }


  public async Task UpdateAsync() {
    var dbContext = ServiceLocator.Get<ModmailDbContext>();
    dbContext.TicketTypes.Update(this);
    await dbContext.SaveChangesAsync();
  }

  public async Task AddAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    await dbContext.TicketTypes.AddAsync(this);
    await dbContext.SaveChangesAsync();
  }

  public static async Task<TicketType?> GetByIdAsync(Guid id) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.TicketTypes.FindAsync(id);
  }

  public static async Task<TicketType?> GetAsync(string keyOrName) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.TicketTypes.FirstOrDefaultAsync(x => x.Key == keyOrName || x.Name == keyOrName);
  }

  public static async Task<bool> ExistsAsync(string keyOrName) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.TicketTypes.AnyAsync(x => x.Name == keyOrName || x.Key == keyOrName);
  }

  public static async Task<List<TicketType>> GetAllAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.TicketTypes.ToListAsync();
  }

  public async Task RemoveAsync() {
    var dbContext = ServiceLocator.Get<ModmailDbContext>();
    dbContext.TicketTypes.Remove(this);
    await dbContext.SaveChangesAsync();
  }

  public static async Task<TicketType?> GetByChannelIdAsync(ulong channelId) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.Tickets.Where(x => x.ModMessageChannelId == channelId)
                          .Select(x => x.TicketType)
                          .FirstOrDefaultAsync();
  }
}
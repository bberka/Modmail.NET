using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;

namespace Modmail.NET.Entities;

public sealed class TicketType
{
  public Guid Id { get; set; }
  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;
  public DateTime? UpdateDateUtc { get; set; }

  public bool IsEnabled { get; set; } = true;

  [MaxLength(DbLength.KEY_STRING)]
  public required string Key { get; set; }

  [MaxLength(DbLength.NAME)]
  public required string Name { get; set; }

  [MaxLength(DbLength.EMOJI)]
  public string? Emoji { get; set; }

  [MaxLength(DbLength.DESCRIPTION)]
  public string Description { get; set; } = string.Empty;

  public int Order { get; set; }

  [MaxLength(DbLength.BOT_MESSAGE)]
  public string? EmbedMessageTitle { get; set; }

  [MaxLength(DbLength.BOT_MESSAGE)]
  public string? EmbedMessageContent { get; set; }


  private async Task UpdateAsync() {
    var dbContext = ServiceLocator.Get<ModmailDbContext>();
    dbContext.TicketTypes.Update(this);
    await dbContext.SaveChangesAsync();
  }

  private async Task AddAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    await dbContext.TicketTypes.AddAsync(this);
    await dbContext.SaveChangesAsync();
  }

  public static async Task<TicketType?> GetByIdAsync(Guid id) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.TicketTypes.FindAsync(id);
  }

  public static async Task<TicketType> GetAsync(string keyOrName) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var result = await dbContext.TicketTypes.FirstOrDefaultAsync(x => x.Key == keyOrName || x.Name == keyOrName);
    if (result is null) throw new NotFoundWithException(LangKeys.TICKET_TYPE, keyOrName);

    return result;
  }

  public static async Task<TicketType?> GetNullableAsync(string keyOrName) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var result = await dbContext.TicketTypes.FirstOrDefaultAsync(x => x.Key == keyOrName || x.Name == keyOrName);
    return result;
  }

  public static async Task<bool> ExistsAsync(string keyOrName) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.TicketTypes.AnyAsync(x => x.Name == keyOrName || x.Key == keyOrName);
  }

  public static async Task<List<TicketType>> GetAllAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var result = await dbContext.TicketTypes.ToListAsync();
    return result;
  }

  public static async Task<List<TicketType>> GetAllActiveAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var result = await dbContext.TicketTypes
                                .Where(x => x.IsEnabled)
                                .ToListAsync();
    return result;
  }


  private async Task RemoveAsync() {
    var dbContext = ServiceLocator.Get<ModmailDbContext>();
    dbContext.TicketTypes.Remove(this);
    await dbContext.SaveChangesAsync();
  }

  public static async Task<TicketType> GetByChannelIdAsync(ulong channelId) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var result = await dbContext.Tickets.Where(x => x.ModMessageChannelId == channelId)
                                .Select(x => x.TicketType)
                                .FirstOrDefaultAsync();
    if (result is null) throw new NotFoundException(LangKeys.TICKET_TYPE);
    return result;
  }

  public static async Task ProcessCreateTicketTypeAsync(string name,
                                                        string? emoji,
                                                        string description,
                                                        long order,
                                                        string? embedMessageTitle,
                                                        string? embedMessageContent) {
    if (string.IsNullOrEmpty(name)) throw new InvalidNameException(name);

    var exists = await ExistsAsync(name);
    if (exists) throw new TicketTypeAlreadyExistsException(name);

    var id = Guid.NewGuid();
    var idClean = id.ToString().Replace("-", "");
    var ticketType = new TicketType {
      Id = id,
      Key = idClean,
      Name = name,
      Emoji = emoji,
      Description = description,
      Order = (int)order,
      RegisterDateUtc = DateTime.UtcNow,
      EmbedMessageTitle = embedMessageTitle,
      EmbedMessageContent = embedMessageContent
    };
    await ticketType.AddAsync();
    //Don't await this task
    _ = Task.Run(async () => {
      var guildOption = await GuildOption.GetAsync();
      if (guildOption.IsEnableDiscordChannelLogging) {
        var logChannel = await ModmailBot.This.GetLogChannelAsync();
        await logChannel.SendMessageAsync(LogResponses.TicketTypeCreated(ticketType));
      }
    });
  }

  public async Task ProcessUpdateTicketTypeAsync() {
    await UpdateAsync();

    _ = Task.Run(async () => {
      var guildOption = await GuildOption.GetAsync();
      if (guildOption.IsEnableDiscordChannelLogging) {
        var logChannel = await ModmailBot.This.GetLogChannelAsync();
        await logChannel.SendMessageAsync(LogResponses.TicketTypeUpdated(this));
      }
    });
  }

  public async Task ProcessRemoveAsync() {
    // var anyActiveTickets = await Ticket.AnyActiveTicketsByTypeAsync(this);
    // if (anyActiveTickets) {
    //   throw new CanNotDeleteTicketTypeWhenActiveTicketsException();
    // }

    var allTicketsByType = await Ticket.GetAllByTypeAsync(this);
    foreach (var ticket in allTicketsByType) {
      ticket.TicketTypeId = null;
      ticket.TicketType = null;
    }

    await Ticket.UpdateRangeAsync(allTicketsByType);
    await RemoveAsync();
    _ = Task.Run(async () => {
      var guildOption = await GuildOption.GetAsync();
      if (guildOption.IsEnableDiscordChannelLogging) {
        var logChannel = await ModmailBot.This.GetLogChannelAsync();
        await logChannel.SendMessageAsync(LogResponses.TicketTypeDeleted(this));
      }
    });
  }
}
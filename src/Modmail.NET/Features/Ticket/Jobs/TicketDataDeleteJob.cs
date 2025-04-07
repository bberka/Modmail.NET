using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Abstract;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database;
using Modmail.NET.Features.Guild.Queries;
using Serilog;

namespace Modmail.NET.Features.Ticket.Jobs;

public class TicketDataDeleteJob : HangfireRecurringJobBase
{
  private readonly IServiceScopeFactory _scopeFactory;

  public TicketDataDeleteJob(IServiceScopeFactory scopeFactory) : base("ticket-data-delete-job", Cron.Daily()) {
    _scopeFactory = scopeFactory;
  }

  public override async Task Execute() {
    var scope = _scopeFactory.CreateScope();

    var sender = scope.ServiceProvider.GetRequiredService<ISender>();

    var guildOption = await sender.Send(new GetGuildOptionQuery(false)) ?? throw new NullReferenceException();
    if (guildOption.TicketDataDeleteWaitDays == -1) return;

    var dbContext = scope.ServiceProvider.GetRequiredService<ModmailDbContext>();
    var timeoutDate = UtilDate.GetNow().AddDays(-guildOption.TicketDataDeleteWaitDays);
    var tickets = await dbContext.Tickets.Where(x => x.RegisterDateUtc < timeoutDate).ToArrayAsync();
    if (tickets.Length > 0) {
      var attachmentIds = await dbContext.TicketMessages
                                         .Include(x => x.Attachments)
                                         .Where(x => x.RegisterDateUtc < timeoutDate)
                                         .SelectMany(x => x.Attachments)
                                         .Select(x => x.Id.ToString())
                                         .ToArrayAsync();


      dbContext.Tickets.RemoveRange(tickets);
      var affected = await dbContext.SaveChangesAsync();
      if (affected == 0) {
        Log.Error("[TicketDataDeleteJob] Database affected rows is 0");
        return;
      }

      Log.Information("[TicketDataDeleteJob] {Count} Ticket data deleted successfully", affected);

      var files = UtilAttachment.GetAllFiles()
                                .Select(x => new {
                                  Path = x,
                                  FileNameWithoutExtension = Path.GetFileNameWithoutExtension(x)
                                })
                                .Where(x => attachmentIds.Contains(x.FileNameWithoutExtension))
                                .Select(x => x.Path)
                                .ToArray();

      foreach (var file in files)
        try {
          File.Delete(file);
        }
        catch (Exception ex) {
          Log.Warning(ex, "[TicketDataDeleteJob] Failed to delete attachment file {File}", file);
        }
    }
  }
}
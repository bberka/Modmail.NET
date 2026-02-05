using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Database;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.Server.Queries;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Language;
using Serilog;

namespace Modmail.NET.Features.Ticket.Jobs;

public class TicketTimeoutJob : HangfireRecurringJobBase
{
    private readonly IServiceScopeFactory _scopeFactory;

    public TicketTimeoutJob(IServiceScopeFactory scopeFactory) : base("ticket-timeout-job", Cron.Hourly())
    {
        _scopeFactory = scopeFactory;
    }

    public override async Task Execute()
    {
        var scope = _scopeFactory.CreateScope();

        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        var bot = scope.ServiceProvider.GetRequiredService<ModmailBot>();

        var guildOption = await sender.Send(new GetOptionQuery()) ?? throw new InvalidOperationException("Guild option not found");
        if (guildOption.TicketTimeoutHours == -1) return; //Disabled

        var tickets = await GetTimeoutTickets();
        if (tickets.Length > 0)
            foreach (var ticket in tickets)
            {
                await sender.Send(new ProcessCloseTicketCommand(bot.Client.CurrentUser.Id, ticket.Id, Lang.TicketTimeout.Translate()));
                Log.Information("Ticket {TicketId} has been closed due to timeout", ticket.Id);
            }

        return;

        async Task<Database.Entities.Ticket[]> GetTimeoutTickets()
        {
            if (guildOption.TicketTimeoutHours == -1) return [];

            var timeoutDate = UtilDate.GetNow()
                .AddHours(-guildOption.TicketTimeoutHours);
            var dbContext = scope.ServiceProvider.GetRequiredService<ModmailDbContext>();
            return await dbContext.Tickets.FilterActive()
                .FilterByLastMessageDateEnd(timeoutDate)
                .ToArrayAsync();
        }
    }
}
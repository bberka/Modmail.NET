using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Database;

namespace Modmail.NET.Features.DiscordCommands.Helpers;

public class TicketTypeProvider : IAutoCompleteProvider
{
    public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext context)
    {
        const string cacheKey = "TicketTypeProvider.Provider.AutoComplete";
        var cache = context.ServiceProvider.GetRequiredService<IMemoryCache>();
        return await cache.GetOrCreateAsync(cacheKey, Get, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
        }) ?? throw new ArgumentNullException(nameof(cacheKey));

        async Task<IEnumerable<DiscordAutoCompleteChoice>> Get(ICacheEntry entry)
        {
            var scope = context.ServiceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ModmailDbContext>();

            var ticketTypesDbList = await dbContext.TicketTypes.ToArrayAsync();
            var ticketTypes = ticketTypesDbList.Select(x => new DiscordAutoCompleteChoice(x.Name, x.Name));
            return await Task.FromResult(ticketTypes.AsEnumerable());
        }
    }
}
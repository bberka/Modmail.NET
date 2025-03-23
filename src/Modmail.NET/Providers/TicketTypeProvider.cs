using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Features.TicketType;

namespace Modmail.NET.Providers;

public class TicketTypeProvider : IAutoCompleteProvider
{
  public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext context) {
    const string cacheKey = "TicketTypeProvider.Provider.AutoComplete";
    var cache = context.ServiceProvider.GetRequiredService<IMemoryCache>();
    return await cache.GetOrCreateAsync(cacheKey, Get, new MemoryCacheEntryOptions {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
    });

    async Task<IEnumerable<DiscordAutoCompleteChoice>> Get(ICacheEntry entry) {
      var scope = context.ServiceProvider.CreateScope();
      var sender = scope.ServiceProvider.GetRequiredService<ISender>();

      var ticketTypesDbList = await sender.Send(new GetTicketTypeListQuery());
      var ticketTypes = ticketTypesDbList.Select(x => new DiscordAutoCompleteChoice(x.Name, x.Name));
      return await Task.FromResult(ticketTypes.AsEnumerable());
    }
  }
}
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Features.TicketType;

namespace Modmail.NET.Providers;

public sealed class TicketTypeProvider : IAutocompleteProvider
{
  public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx) {
    const string cacheKey = "TicketTypeProvider.Provider.AutoComplete";
    var cache = ctx.Services.GetRequiredService<IMemoryCache>();
    return await cache.GetOrCreateAsync(cacheKey, Get, new MemoryCacheEntryOptions {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
    });

    async Task<IEnumerable<DiscordAutoCompleteChoice>> Get(ICacheEntry entry) {
      var scope = ctx.Services.CreateScope();
      var sender = scope.ServiceProvider.GetRequiredService<ISender>();

      var ticketTypesDbList = await sender.Send(new GetTicketTypeListQuery());
      var ticketTypes = ticketTypesDbList.Select(x => new DiscordAutoCompleteChoice(x.Name, x.Name));
      return await Task.FromResult(ticketTypes.AsEnumerable());
    }
  }
}
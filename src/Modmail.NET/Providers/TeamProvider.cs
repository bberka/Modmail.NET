using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Features.Teams;

namespace Modmail.NET.Providers;

public sealed class TeamProvider : IAutocompleteProvider
{
  public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx) {
    const string cacheKey = "TeamProvider.Provider.AutoComplete";
    var cache = ctx.Services.GetRequiredService<IMemoryCache>();
    return await cache.GetOrCreateAsync(cacheKey, Get, new MemoryCacheEntryOptions {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
    });

    async Task<IEnumerable<DiscordAutoCompleteChoice>> Get(ICacheEntry cacheEntry) {
      var scope = ctx.Services.CreateScope();
      var sender = scope.ServiceProvider.GetRequiredService<ISender>();
      var teamsDbList = await sender.Send(new GetTeamListQuery());
      var teams = teamsDbList.Select(x => new DiscordAutoCompleteChoice(x.Name, x.Name));
      return await Task.FromResult(teams.AsEnumerable());
    }
  }
}
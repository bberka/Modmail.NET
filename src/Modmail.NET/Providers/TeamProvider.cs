using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Features.Teams;

namespace Modmail.NET.Providers;

public class TeamProvider : IAutoCompleteProvider
{
  public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext context) {
    const string cacheKey = "TeamProvider.Provider.AutoComplete";
    var cache = context.ServiceProvider.GetRequiredService<IMemoryCache>();
    return await cache.GetOrCreateAsync(cacheKey, Get, new MemoryCacheEntryOptions {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
    });

    async Task<IEnumerable<DiscordAutoCompleteChoice>> Get(ICacheEntry cacheEntry) {
      var scope = context.ServiceProvider.CreateScope();
      var sender = scope.ServiceProvider.GetRequiredService<ISender>();
      var teamsDbList = await sender.Send(new GetTeamListQuery());
      var teams = teamsDbList.Select(x => new DiscordAutoCompleteChoice(x.Name, x.Name));
      return await Task.FromResult(teams.AsEnumerable());
    }
  }
}
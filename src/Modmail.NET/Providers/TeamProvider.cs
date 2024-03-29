using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Aspects;
using Modmail.NET.Entities;

namespace Modmail.NET.Providers;

public class TeamProvider : IAutocompleteProvider
{
  [CacheAspect(CacheSeconds = 3)]
  public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx) {
    var teamsDbList = await GuildTeam.GetAllAsync();
    var teams = teamsDbList.Select(x => new DiscordAutoCompleteChoice(x.Name, x.Name));
    return await Task.FromResult(teams.AsEnumerable());
  }
}
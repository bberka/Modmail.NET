using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Entities;

namespace Modmail.NET.Providers;

public class TeamProvider : IAutocompleteProvider
{
  public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx) {
    var key = SimpleCacher.CreateKey(nameof(TeamProvider), nameof(Provider));
    return await SimpleCacher.Instance.GetOrSetAsync(key, _get, TimeSpan.FromSeconds(60)) ?? await _get();

    async Task<IEnumerable<DiscordAutoCompleteChoice>> _get() {
      var teamsDbList = await GuildTeam.GetAllAsync();
      var teams = teamsDbList.Select(x => new DiscordAutoCompleteChoice(x.Name, x.Name));
      return await Task.FromResult(teams.AsEnumerable());
    }
  }
}
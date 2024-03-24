using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Entities;

namespace Modmail.NET.Providers;

public class TeamProvider : IAutocompleteProvider
{
  public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx) {
    var teamsDbList = await GuildTeam.GetAllAsync(ctx.Guild.Id);
    var teams = teamsDbList.Select(x => new DiscordAutoCompleteChoice(x.Name, x.Name));
    return await Task.FromResult(teams.AsEnumerable());
  }
}
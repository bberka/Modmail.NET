using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Common;

namespace Modmail.NET.Providers;

public class TeamProvider : IAutocompleteProvider
{
  public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx) {
    var dbService = ServiceLocator.Get<IDbService>();
    var teamsDbList = await dbService.GetTeamsAsync(ctx.Guild.Id);
    var teams = teamsDbList.Select(x => new DiscordAutoCompleteChoice(x.Name, x.Name));
    return await Task.FromResult(teams.AsEnumerable());
  }
}
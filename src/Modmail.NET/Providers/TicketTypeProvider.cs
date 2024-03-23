using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Common;

namespace Modmail.NET.Providers;

public class TicketTypeProvider : IAutocompleteProvider
{
  public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx) {
    var dbService = ServiceLocator.Get<IDbService>();
    var ticketTypesDbList = await dbService.GetEnabledTicketTypesAsync();
    var ticketTypes = ticketTypesDbList.Select(x => new DiscordAutoCompleteChoice(x.Name, x.Name));
    return await Task.FromResult(ticketTypes.AsEnumerable());
  }
}
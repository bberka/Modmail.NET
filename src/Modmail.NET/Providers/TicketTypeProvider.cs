using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Entities;

namespace Modmail.NET.Providers;

public class TicketTypeProvider : IAutocompleteProvider
{
  public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx) {
    var key = SimpleCacher.CreateKey(nameof(TicketTypeProvider), nameof(Provider));
    return await SimpleCacher.Instance.GetOrSetAsync(key, _get, TimeSpan.FromSeconds(60)) ?? await _get();

    async Task<IEnumerable<DiscordAutoCompleteChoice>> _get() {
      var ticketTypesDbList = await TicketType.GetAllAsync();
      var ticketTypes = ticketTypesDbList.Select(x => new DiscordAutoCompleteChoice(x.Name, x.Name));
      return await Task.FromResult(ticketTypes.AsEnumerable());
    }
  }
}
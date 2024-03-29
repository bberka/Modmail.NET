﻿using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Aspects;
using Modmail.NET.Entities;

namespace Modmail.NET.Providers;

public class TicketTypeProvider : IAutocompleteProvider
{
  [CacheAspect(CacheSeconds = 3)]
  public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx) {
    var ticketTypesDbList = await TicketType.GetAllAsync();
    var ticketTypes = ticketTypesDbList.Select(x => new DiscordAutoCompleteChoice(x.Name, x.Name));
    return await Task.FromResult(ticketTypes.AsEnumerable());
  }
}
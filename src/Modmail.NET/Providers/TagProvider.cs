using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Common;

namespace Modmail.NET.Providers;

// public class TagProvider : IAutocompleteProvider
// {
//   public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx) {
//     var dbService = ServiceLocator.Get<IDbService>();
//     var tagsDbList = await dbService.GetTagsAsync(ctx.Guild.Id);
//     var tags = tagsDbList.Select(x => new DiscordAutoCompleteChoice(x.Key, x.Key));
//
//     return await Task.FromResult(tags.AsEnumerable());
//   }
// }
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Abstract.Services;

namespace Modmail.NET.Common;

public class TagChoiceProvider : ChoiceProvider
{
  public override async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider() {
    if (!GuildId.HasValue) return await Task.FromResult(Enumerable.Empty<DiscordApplicationCommandOptionChoice>());

    var dbService = ServiceLocator.Get<IDbService>();
    var tagsDbList = await dbService.GetTagsAsync(GuildId.Value);
    var tags = tagsDbList.Select(x => new DiscordApplicationCommandOptionChoice(x.Key, x.Key));

    return await Task.FromResult(tags.AsEnumerable());
  }
}
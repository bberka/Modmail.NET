using DSharpPlus.Entities;
using MediatR;
using Microsoft.Extensions.Options;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;
using Modmail.NET.Features.UserInfo;
using Serilog;

namespace Modmail.NET.Features.Bot.Handlers;

public class GetDiscordMainGuildHandler : IRequestHandler<GetDiscordMainGuildQuery, DiscordGuild>
{
  private readonly ModmailBot _bot;
  private readonly ISender _sender;
  private readonly ModmailDbContext _dbContext;
  private readonly IOptions<BotConfig> _options;

  public GetDiscordMainGuildHandler(ModmailBot bot,
                                    ISender sender,
                                    ModmailDbContext dbContext,
                                    IOptions<BotConfig> options) {
    _bot = bot;
    _sender = sender;
    _dbContext = dbContext;
    _options = options;
  }
  public async Task<DiscordGuild> Handle(GetDiscordMainGuildQuery request, CancellationToken cancellationToken) {
    var guildId = _options.Value.MainServerId;
    var guild = await _bot.Client.GetGuildAsync(guildId);
    if (guild == null) {
      Log.Error("Main guild not found: {GuildId}", guildId);
      throw new NotFoundException(LangKeys.MAIN_GUILD);
    }

    var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken) ?? throw new NullReferenceException();
    var isSame = guildOption.Name == guild.Name && guildOption.IconUrl == guild.IconUrl && guildOption.BannerUrl == guild.BannerUrl;
    if (isSame) {
      return guild;
    }
    
    guildOption.Name = guild.Name;
    guildOption.IconUrl = guild.IconUrl;
    guildOption.BannerUrl = guild.BannerUrl;

    _dbContext.Update(guildOption); 
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    var user = await guild.GetMemberAsync(guild.OwnerId);
    await _sender.Send(new UpdateDiscordUserCommand(user), cancellationToken);
    return guild;
  }
}
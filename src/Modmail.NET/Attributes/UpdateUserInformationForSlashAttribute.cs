﻿using DSharpPlus.SlashCommands;
using Modmail.NET.Entities;

namespace Modmail.NET.Attributes;

public class UpdateUserInformationForSlashAttribute : SlashCheckBaseAttribute
{
  public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx) {
    await DiscordUserInfo.AddOrUpdateAsync(ctx?.User);
    return true;
  }
}
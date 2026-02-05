using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Features.User.Commands;

namespace Modmail.NET.Features.DiscordBot.Events;

public static class UserUpdateEvents
{
    public static async Task UpdateUser(DiscordClient client, DiscordUser? user)
    {
        if (user is null) return;

        var scope = client.ServiceProvider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        await sender.Send(new UpdateDiscordUserCommand(user));
    }

    public static async Task InteractionCreated(DiscordClient client, InteractionCreatedEventArgs args)
    {
        await UpdateUser(client, args.Interaction.User);
    }

    public static async Task OnUserUpdated(DiscordClient client, UserUpdatedEventArgs args)
    {
        await UpdateUser(client, args.UserAfter);
    }

    public static async Task OnUserSettingsUpdated(DiscordClient client, UserSettingsUpdatedEventArgs args)
    {
        await UpdateUser(client, args.User);
    }

    public static async Task OnGuildMemberAdded(DiscordClient client, GuildMemberAddedEventArgs args)
    {
        await UpdateUser(client, args.Member);
    }

    public static async Task OnGuildMemberRemoved(DiscordClient client, GuildMemberRemovedEventArgs args)
    {
        await UpdateUser(client, args.Member);
    }

    public static async Task OnGuildBanAdded(DiscordClient client, GuildBanAddedEventArgs args)
    {
        await UpdateUser(client, args.Member);
    }

    public static async Task OnGuildBanRemoved(DiscordClient client, GuildBanRemovedEventArgs args)
    {
        await UpdateUser(client, args.Member);
    }
}
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Common;
using Modmail.NET.Static;

namespace Modmail.NET.Events;

public static class ModalSubmitted
{
  public static async Task Handle(DiscordClient sender, ModalSubmitEventArgs args) {
    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral().WithContent(Texts.THANK_YOU_FOR_FEEDBACK));

    var interaction = args.Interaction;
    var id = interaction.Data.CustomId;
    var (interactionName, parameters) = UtilInteraction.ParseKey(id);


    switch (interactionName) {
      case "feedback":
        var textInput = args.Values["feedback"];

        var starParam = parameters[0];
        var ticketIdParam = parameters[1];
        var messageIdParam = parameters[2];

        var starCount = int.Parse(starParam);
        var ticketId = Guid.Parse(ticketIdParam);
        var messageId = ulong.Parse(messageIdParam);

        var dbService = ServiceLocator.Get<IDbService>();

        var guildOption = await dbService.GetOptionAsync(MMConfig.This.MainServerId);
        if (guildOption is null) throw new InvalidOperationException("Guild option not found: " + MMConfig.This.MainServerId);

        if (!guildOption.TakeFeedbackAfterClosing) throw new InvalidOperationException("Feedback is not enabled for this guild: " + MMConfig.This.MainServerId);


        var mainGuild = await ModmailBot.This.Client.GetGuildAsync(MMConfig.This.MainServerId);


        await dbService.AddFeedbackAsync(ticketId, starCount, textInput);

        var message = await args.Interaction.Channel.GetMessageAsync(messageId);
        var feedbackDoneEmbed = ModmailEmbeds.Interaction.EmbedFeedbackDone(starCount, textInput, mainGuild);
        await message.ModifyAsync(x => { x.AddEmbed(feedbackDoneEmbed); });

        var logChannel = mainGuild.GetChannel(guildOption.LogChannelId);
        if (logChannel is not null) {
          var logEmbed = ModmailEmbeds.ToLog.FeedbackReceived(starCount, textInput, mainGuild, args.Interaction.User);
          await logChannel.SendMessageAsync(logEmbed);
        }

        break;
    }

    await Task.CompletedTask;
  }
}
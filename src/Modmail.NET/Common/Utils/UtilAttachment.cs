using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Ticket.Services;

namespace Modmail.NET.Common.Utils;

public static class UtilAttachment
{
  public static string GetUri(Guid id) {
    var botConfig = ServiceLocator.GetBotConfig();
    if (string.IsNullOrEmpty(botConfig.Domain)) throw new InvalidOperationException();

    var uri = new Uri(botConfig.Domain, UriKind.Absolute);
    return uri + "attachments/" + id;
  }

  public static string GetLocalPath(TicketMessageAttachment attachment) {
    var uri = new Uri(TicketAttachmentDownloadService.AttachmentDownloadDirectory, UriKind.RelativeOrAbsolute);
    return uri + "/" + attachment.Id + Path.GetExtension(attachment.FileName);
  }

  public static string[] GetAllFiles() {
    return !Directory.Exists(TicketAttachmentDownloadService.AttachmentDownloadDirectory)
             ? []
             : Directory.GetFiles(TicketAttachmentDownloadService.AttachmentDownloadDirectory);
  }
}
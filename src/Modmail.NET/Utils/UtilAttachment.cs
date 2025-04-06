using Modmail.NET.Entities;

namespace Modmail.NET.Utils;

public static class UtilAttachment
{
  public static string GetUri(Guid id) {
    var botConfig = ServiceLocator.GetBotConfig();
    if (string.IsNullOrEmpty(botConfig.Domain)) throw new InvalidOperationException();

    var uri = new Uri(botConfig.Domain, UriKind.Absolute);
    return uri + "attachments/" + id;
  }

  public static string GetLocalPath(TicketMessageAttachment attachment) {
    var uri = new Uri(Const.AttachmentDownloadDirectory, UriKind.RelativeOrAbsolute);
    return uri + "/" + attachment.Id + Path.GetExtension(attachment.FileName);
  }

  public static string[] GetAllFiles() {
    return !Directory.Exists(Const.AttachmentDownloadDirectory)
             ? []
             : Directory.GetFiles(Const.AttachmentDownloadDirectory);
  }
}
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Features.Ticket.Services;

namespace Modmail.NET.Features.Ticket.Handlers;

public class NotifyTicketMessageSentAttachmentDownloadHandler : INotificationHandler<NotifyTicketMessageSent>
{
	private readonly TicketAttachmentDownloadService _attachmentDownloadService;

	public NotifyTicketMessageSentAttachmentDownloadHandler(TicketAttachmentDownloadService attachmentDownloadService) {
		_attachmentDownloadService = attachmentDownloadService;
	}

	public async ValueTask Handle(NotifyTicketMessageSent notification, CancellationToken cancellationToken) {
		var tasks = notification.Message.Attachments.Select(attachment => _attachmentDownloadService.Handle(attachment.Id, attachment.Url, Path.GetExtension(attachment.FileName))).ToArray();
		await Task.WhenAll(tasks);
	}
}
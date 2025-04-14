using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Features.Ticket.Services;

namespace Modmail.NET.Web.Blazor.Controllers;

[Route("attachments")]
public class AttachmentController : Controller
{
  private readonly ModmailDbContext _dbContext;

  public AttachmentController(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  [HttpGet("{id:guid}")]
  public async Task<IActionResult> Get(Guid id) {
    if (id == Guid.Empty) return NotFound();

    var attachment = await _dbContext.MessageAttachments.Where(x => x.Id == id)
                                     .Select(x => new {
                                       x.MediaType,
                                       x.FileName
                                     })
                                     .FirstOrDefaultAsync();
    if (attachment is null) return NotFound();

    var extension = Path.GetExtension(attachment.FileName) ?? throw new NullReferenceException("extension"); //starts with .
    var filePath = Path.Combine(TicketAttachmentDownloadService.AttachmentDownloadDirectory, id + extension);
    var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
    return new FileStreamResult(stream, attachment.MediaType);
  }
}
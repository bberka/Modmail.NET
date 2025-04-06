using Serilog;

namespace Modmail.NET.Services;

public class TicketAttachmentDownloadService
{
  private readonly IHttpClientFactory _httpClientFactory;

  public TicketAttachmentDownloadService(IHttpClientFactory httpClientFactory) {
    _httpClientFactory = httpClientFactory;
  }

  public async Task Handle(Guid attachmentId, string url, string fileExtension) {
    if (attachmentId == Guid.Empty) throw new ArgumentNullException(nameof(attachmentId));

    if (!Directory.Exists(Const.AttachmentDownloadDirectory)) Directory.CreateDirectory(Const.AttachmentDownloadDirectory);

    var filePath = Path.Combine(Const.AttachmentDownloadDirectory, $"{attachmentId}.{fileExtension.Trim().Trim('.')}");
    if (Path.Exists(filePath)) throw new InvalidOperationException("File in path already exists: " + filePath);

    var client = _httpClientFactory.CreateClient();
    client.Timeout = TimeSpan.FromSeconds(Const.HttpClientDownloadTimeoutSeconds);

    try {
      using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead)) {
        response.EnsureSuccessStatusCode();
        await using (var streamToReadFrom = await response.Content.ReadAsStreamAsync()) {
          await using (var streamToWriteTo = File.Open(filePath, FileMode.Create)) {
            await streamToReadFrom.CopyToAsync(streamToWriteTo);
          }
        }
      }

      Log.Information("[TicketAttachmentDownloadService] File downloaded successfully to {FilePath}", filePath);
    }
    catch (HttpRequestException ex) {
      Log.Error(ex, "[TicketAttachmentDownloadService] Error downloading file from {Url}: {ExMessage}", url, ex.Message);
    }
    catch (Exception ex) {
      Log.Fatal(ex, "[TicketAttachmentDownloadService] An exception occurred: {ExMessage}", ex.Message);
    }
  }
}
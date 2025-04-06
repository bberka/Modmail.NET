using Serilog;

namespace Modmail.NET.Queues;

public static class TicketAttachmentDownloadQueueHandler
{
  public static async Task Handle(Guid attachmentId, string url, string fileExtension) {
    if (!Directory.Exists(Const.AttachmentDownloadDirectory)) Directory.CreateDirectory(Const.AttachmentDownloadDirectory);

    var filePath = Path.Combine(Const.AttachmentDownloadDirectory, $"{attachmentId}.{fileExtension}");
    if (Path.Exists(filePath)) throw new InvalidOperationException("File in path already exists: " + filePath);

    var httpClient = ServiceLocator.CreateHttpClient();
    httpClient.Timeout = TimeSpan.FromSeconds(Const.HttpClientDownloadTimeoutSeconds);

    try {
      using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead)) {
        response.EnsureSuccessStatusCode();
        await using (var streamToReadFrom = await response.Content.ReadAsStreamAsync()) {
          await using (var streamToWriteTo = File.Open(filePath, FileMode.Create)) {
            await streamToReadFrom.CopyToAsync(streamToWriteTo);
          }
        }
      }

      Log.Information("File downloaded successfully to {FilePath}", filePath);
    }
    catch (HttpRequestException ex) {
      Log.Error(ex, "Error downloading file from {Url}: {ExMessage}", url, ex.Message);
    }
    catch (Exception ex) {
      Log.Fatal(ex, "An exception occurred: {ExMessage}", ex.Message);
    }
    finally {
      httpClient?.Dispose();
    }
  }
}
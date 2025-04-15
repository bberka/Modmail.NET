using Serilog;

namespace Modmail.NET.Common.Utils;

public static class UtilTranscript
{
	public static Uri? GetTranscriptUri(Guid ticketId) {
		var config = ServiceLocator.GetBotConfig();
		try {
			return new Uri(config.DomainUri, "transcript/" + ticketId);
		}
		catch (UriFormatException ex) {
			Log.Error(ex, "Invalid domain format: {Domain} {TranscriptUri}", config.DomainUri, "transcript/" + ticketId);
			return null;
		}
	}
}
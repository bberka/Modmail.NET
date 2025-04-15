using Modmail.NET.Common.Exceptions;
using Serilog;

namespace Modmail.NET.Pipeline;

public class LoggerPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IMessage
{
	public async ValueTask<TResponse> Handle(TRequest message, CancellationToken cancellationToken, MessageHandlerDelegate<TRequest, TResponse> next) {
		var reqName = typeof(TRequest).Name;
		try {
			var response = await next(message, cancellationToken);
			Log.Verbose("[MEDIATOR] [{ReqName}] SUCCESS", reqName);
			return response;
		}
		catch (ModmailBotException ex) {
			Log.Verbose(ex, "[MEDIATOR] [{ReqName}] ERROR", reqName);
			throw;
		}
		catch (Exception ex) {
			Log.Error(ex, "[MEDIATOR] [{ReqName}] EXCEPTION", reqName);
			throw;
		}
	}
}
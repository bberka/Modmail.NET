using MediatR;
using Modmail.NET.Common.Exceptions;
using Serilog;

namespace Modmail.NET.Pipeline;

public class LoggerPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
    var reqName = typeof(TRequest).Name;
    try {
      var response = await next();
      Log.Verbose("[MEDIATR] [{ReqName}] RUN FINISHED", reqName);
      return response;
    }
    catch (ModmailBotException ex) {
      Log.Verbose(ex, "[MEDIATR] [{ReqName}] ERROR", reqName);
      throw;
    }
    catch (Exception ex) {
      Log.Error(ex, "[MEDIATR] [{ReqName}] EXCEPTION", reqName);
      throw;
    }
  }
}
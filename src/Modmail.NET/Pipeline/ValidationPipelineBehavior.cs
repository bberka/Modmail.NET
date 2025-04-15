using FluentValidation;

namespace Modmail.NET.Pipeline;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;

	public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) {
		_validators = validators;
	}

	public async ValueTask<TResponse> Handle(TRequest message, CancellationToken cancellationToken, MessageHandlerDelegate<TRequest, TResponse> next) {
		if (!_validators.Any())
			return await next(message, cancellationToken);

		var context = new ValidationContext<TRequest>(message);
		var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
		var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

		if (failures.Count != 0)
			throw new ValidationException(failures);

		return await next(message, cancellationToken);
	}
}
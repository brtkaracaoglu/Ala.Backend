using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.SystemMessages;
using FluentValidation;
using MediatR;

namespace Ala.Backend.Application.Common.Behaviors
{
    public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators is null || !_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null && !string.IsNullOrWhiteSpace(f.ErrorMessage))
                .ToList();

            if (!failures.Any())
                return await next();

            var errorDict = failures
                .GroupBy(f => f.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage)
                          .Distinct()
                          .ToArray()
                );

            throw new AppValidationException(detail: Response.Common.ValidationFailed, errors: errorDict);
        }
    }
}
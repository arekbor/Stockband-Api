using FluentValidation;
using MediatR;
using Stockband.Domain.Common;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Stockband.Application.Behaviors;

public class ValidationPipelineBehavior<TRequest, TResponse>:IPipelineBehavior<TRequest,TResponse>
    where TRequest:IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }
        
        IEnumerable<ValidationResult> validationResults = _validators
            .Select(x => x.Validate(request))
            .Where(x => x.IsValid == false)
            .ToList();

        if (!validationResults.Any())
        {
            return await next();
        }

        return (TResponse)Activator.CreateInstance(typeof(TResponse), 
            validationResults, BaseErrorCode.FluentValidationCode)!;
    }
}
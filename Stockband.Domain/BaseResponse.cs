using FluentValidation.Results;
using Stockband.Domain.Common;

namespace Stockband.Domain;

public class BaseResponse
{
    public List<BaseError> Errors { get; } = new();

    public bool Success => !Errors.Any();

    public BaseResponse()
    {
        
    }
    
    public BaseResponse(Exception exception, BaseErrorCode code = BaseErrorCode.DefaultCode)
    {
        BaseError baseError = new BaseError
        {
            Message = exception.Message,
            Code = code
        };
        Errors.Add(baseError);
    }
    
    public BaseResponse(ValidationResult validationResult, BaseErrorCode code = BaseErrorCode.FluentValidationCode)
    {
        foreach (ValidationFailure validationFailure in validationResult.Errors)
        {
            BaseError baseError = new BaseError
            {
                Message = validationFailure.ErrorMessage,
                Code = code
            };
            Errors.Add(baseError);
        }
    }
}

public class BaseResponse<T> : BaseResponse
{
    public T Result { get; }

    public BaseResponse()
    {
        
    }
    
    public BaseResponse(T result)
    {
        Result = result;
    }

    public BaseResponse(Exception exception, BaseErrorCode code = BaseErrorCode.DefaultCode)
        :base(exception)
    {
        
    }

    public BaseResponse(ValidationResult validationResult, BaseErrorCode code = BaseErrorCode.FluentValidationCode)
        :base(validationResult)
    {
        
    }
}
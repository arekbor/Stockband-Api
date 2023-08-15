using FluentValidation.Results;
using Stockband.Domain.Enums;

namespace Stockband.Domain.Common;

public class BaseResponse
{
    public List<BaseError> Errors { get; } = new();

    public bool Success => !Errors.Any();

    public BaseResponse()
    {
        
    }

    public BaseResponse(List<BaseError> errors)
    {
        Errors.AddRange(errors);
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

    public BaseResponse(IEnumerable<ValidationResult> validationResults,
        BaseErrorCode code = BaseErrorCode.FluentValidationCode)
    {
        List<BaseError> errors = new List<BaseError>();
        
        foreach (ValidationResult validationResult in validationResults)
        {
            foreach (ValidationFailure validationFailure in validationResult.Errors)
            {
                BaseError baseError = new BaseError
                {
                    Message = validationFailure.ErrorMessage,
                    Code = code
                };
                errors.Add(baseError);
            }
        }
        Errors.AddRange(errors);
    }
}

public class BaseResponse<T> : BaseResponse
{
    public T Result { get; set; }

    public BaseResponse()
    {
        
    }

    public BaseResponse(List<BaseError> errors)
        :base(errors)
    {
        
    }
    
    public BaseResponse(T result)
    {
        Result = result;
    }

    public BaseResponse(Exception exception, BaseErrorCode code = BaseErrorCode.DefaultCode)
        :base(exception, code)
    {
        
    }

    public BaseResponse(ValidationResult validationResult, BaseErrorCode code = BaseErrorCode.FluentValidationCode)
        :base(validationResult, code)
    {
        
    }

    public BaseResponse(IEnumerable<ValidationResult> validationResults,
        BaseErrorCode code = BaseErrorCode.FluentValidationCode)
        :base(validationResults, code)
    {
        
    }
}
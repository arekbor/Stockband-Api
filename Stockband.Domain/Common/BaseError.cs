using Stockband.Domain.Enums;

namespace Stockband.Domain.Common;

public class BaseError
{
    public string Message { get; set; }
    public BaseErrorCode Code { get; set; }
}
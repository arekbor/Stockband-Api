namespace Stockband.Domain.Exceptions;

public class PerformRestrictedOperationException:Exception
{
    public PerformRestrictedOperationException()
        :base("Unable to perform this operation")
    {
        
    }
}
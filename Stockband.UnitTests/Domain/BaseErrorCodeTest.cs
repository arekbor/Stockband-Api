using NUnit.Framework;
using Stockband.Domain.Enums;

namespace Stockband.UnitTests.Domain;

public class BaseErrorCodeTest
{
    [Test]
    public void BaseErrorCode_ShouldBeInCorrectOrder()
    {
        BaseErrorCode[] baseErrorCodesValues = (BaseErrorCode[])Enum.GetValues(typeof(BaseErrorCode));

        for (int i = 0; i < baseErrorCodesValues.Length -1; i++)
        {
            Assert.Less(baseErrorCodesValues[i], baseErrorCodesValues[i + 1]);
        }
    }
}
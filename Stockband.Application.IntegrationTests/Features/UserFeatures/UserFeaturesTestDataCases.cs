using Bogus;

namespace Stockband.Application.IntegrationTests.Features.UserFeatures;

internal class UserFeaturesTestDataCases
{
    internal static object[] InvalidUsernameOrEmailCases =
    {
        new object[] { new Faker().Lorem.Sentence(200), new Faker().Person.Email},
        new object[] { new Faker().Person.UserName, new Faker().Lorem.Sentence(200)},
    };

    internal static object[] InvalidEmailOrPasswordCases =
    {
        new object[] { new Faker().Lorem.Sentence(2), new Faker().Internet.Password(20)},
        new object[] { new Faker().Person.Email, String.Empty},
    };
}
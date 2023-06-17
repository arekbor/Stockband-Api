using Bogus;

namespace Stockband.Application.IntegrationTests.Features.ProjectFeatures;

internal static class ProjectFeaturesTestCasesData
{
    internal static object[] InvalidProjectNamesAndDescriptionsCases =
    {
        new object[] { new Faker().Lorem.Sentence(200), new Faker().Lorem.Sentence(200)},
        new object[] { new Faker().Lorem.Sentence(200), new Faker().Lorem.Sentence(200)},
    };
    
    internal static object[] InvalidProjectNamesOrDescriptionsCases =
    {
        new object[] { new Faker().Lorem.Sentence(200), new Faker().Lorem.Sentence(2)},
        new object[] { new Faker().Lorem.Sentence(2), new Faker().Lorem.Sentence(200)},
    };
}
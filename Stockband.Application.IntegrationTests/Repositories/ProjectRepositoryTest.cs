using Bogus;
using FizzWare.NBuilder;
using NUnit.Framework;
using Shouldly;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Application.IntegrationTests.Repositories;

[TestFixture]
public class ProjectRepositoryTest:BaseTest
{
    private IProjectRepository _projectRepository = null!;
    [SetUp]
    public void SetUp()
    {
        _projectRepository = new ProjectRepository(Context);
    }

    [Test]
    public async Task GetProjectByNameAsync_ShouldReturnObject()
    {
        //Arrange
        string testingProjectName = new Faker().Lorem.Sentence(1);

        List<Project> projectsForTest = Builder<Project>
            .CreateListOfSize(100)
            .All()
            .With(x => x.Deleted = false)
            .TheFirst(1)
            .With(x => x.Name = testingProjectName)
            .Build()
            .ToList();
        await _projectRepository.AddRangeAsync(projectsForTest);

        //Act
        Project? result = await _projectRepository.GetProjectByNameAsync(testingProjectName);
        if (result == null)
        {
            throw new ObjectNotFound(typeof(Project));
        }

        //Assert
        result.Name.ShouldBe(testingProjectName);
    }

    [Test]
    public async Task GetAllProjectsByOwnerId_ShouldReturnObject()
    {
        //Assert
        const int testingOwnerId = 520;
        const int amount = 15;
        
        List<Project> projectsForTest = Builder<Project>
            .CreateListOfSize(200)
            .All()
            .With(x => x.Deleted = false)
            .TheFirst(amount)
            .With(x => x.OwnerId = testingOwnerId)
            .Build()
            .ToList();
        await _projectRepository.AddRangeAsync(projectsForTest);
        
        //Act
        IEnumerable<Project> result = await _projectRepository.GetAllProjectsByOwnerId(testingOwnerId);

        //Assert
        result.Count().ShouldBe(amount);
    }
}
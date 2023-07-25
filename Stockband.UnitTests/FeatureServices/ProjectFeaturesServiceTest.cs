using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using Shouldly;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.FeatureServices;
using Stockband.Application.Interfaces.ExternalServices;
using Stockband.Domain.Entities;

namespace Stockband.UnitTests.FeatureServices;

public class ProjectFeaturesServiceTest
{
    private Mock<IProjectRepository> _projectRepositoryMock = null!;
    private Mock<IConfigurationHelperService> _configurationHelperService = null!;

    private IProjectFeaturesService _projectFeaturesService = null!;
    
    [SetUp]
    public void SetUp()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _configurationHelperService = new Mock<IConfigurationHelperService>();
    }

    [Test]
    [TestCase(10)]
    [TestCase(15)]
    public async Task IsProjectsLimitExceeded_ShouldReturnTrue(int amountOfProjects)
    {
        //Arrange
        const int limit = 10;
        SetupMocksIsProjectsLimitExceeded(amountOfProjects, limit);
        
        //Act
        bool result = await _projectFeaturesService.IsProjectsLimitExceeded(1);
        
        //Assert
        result.ShouldBe(true);
    }
    
    [Test]
    [TestCase(15)]
    [TestCase(19)]
    public async Task IsProjectsLimitExceeded_ShouldReturnFalse(int amountOfProjects)
    {
        //Arrange
        const int limit = 20;
        SetupMocksIsProjectsLimitExceeded(amountOfProjects, limit);
        
        //Act
        bool result = await _projectFeaturesService.IsProjectsLimitExceeded(1);
        
        //Assert
        result.ShouldBe(false);
    }

    private void SetupMocksIsProjectsLimitExceeded(int amountOfProjects, int limit)
    {
        _projectRepositoryMock
            .Setup(x => x.GetAllProjectsByOwnerId(It.IsAny<int>()))
            .ReturnsAsync(Builder<Project>
                .CreateListOfSize(amountOfProjects)
                .All()
                .With(x => x.Deleted = false)
                .Build()
                .ToList());

        _configurationHelperService
            .Setup(x => x.GetProjectsLimitPerUser())
            .Returns(limit);
        
        _projectFeaturesService = new ProjectFeaturesService(_configurationHelperService.Object, _projectRepositoryMock.Object);
    }
}
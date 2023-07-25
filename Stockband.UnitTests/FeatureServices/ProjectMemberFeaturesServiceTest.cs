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

public class ProjectMemberFeaturesServiceTest
{
    private Mock<IConfigurationHelperService> _configurationHelperService = null!;
    private Mock<IProjectMemberRepository> _projectMemberRepositoryMock = null!;

    private IProjectMemberFeaturesService _projectMemberFeaturesService = null!;

    [SetUp]
    public void SetUp()
    {
        _configurationHelperService = new Mock<IConfigurationHelperService>();
        _projectMemberRepositoryMock = new Mock<IProjectMemberRepository>();
    }

    [Test]
    [TestCase(10)]
    [TestCase(15)]
    public async Task IsProjectMembersLimitExceeded_ShouldReturnTrue(int amountOfProjectMembers)
    {
        //Arrange
        const int limit = 10;
        SetupMocksIsProjectMembersLimitExceeded(amountOfProjectMembers, limit);

        //Act
        bool result = await _projectMemberFeaturesService.IsProjectMembersLimitExceeded(1);

        //Assert
        result.ShouldBe(true);
    }
    
    [Test]
    [TestCase(20)]
    [TestCase(24)]
    public async Task IsProjectMembersLimitExceeded_ShouldReturnFalse(int amountOfProjectMembers)
    {
        //Arrange
        const int limit = 25;
        SetupMocksIsProjectMembersLimitExceeded(amountOfProjectMembers, limit);
        
        //Act
        bool result = await _projectMemberFeaturesService.IsProjectMembersLimitExceeded(1);

        //Assert
        result.ShouldBe(false);
    }

    private void SetupMocksIsProjectMembersLimitExceeded(int amountOfProjectMembers, int limit)
    {
        List<ProjectMember> projectMembers = Builder<ProjectMember>
            .CreateListOfSize(amountOfProjectMembers)
            .All()
            .With(x => x.Deleted = false)
            .Build()
            .ToList();

        _projectMemberRepositoryMock
            .Setup(x => x.GetAllProjectMembersByProjectIdAsync(It.IsAny<int>()))
            .ReturnsAsync(projectMembers);

        _configurationHelperService
            .Setup(x => x.GetProjectMembersLimitPerProject())
            .Returns(limit);

        _projectMemberFeaturesService = new ProjectMemberFeaturesService
            (_configurationHelperService.Object, _projectMemberRepositoryMock.Object);
    }
}
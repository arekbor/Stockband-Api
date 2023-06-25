using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using Shouldly;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.FeatureServices;
using Stockband.Application.Interfaces.CommonServices;
using Stockband.Domain.Entities;

namespace Stockband.Application.UnitTests.FeatureServices;

public class ProjectMemberFeaturesServiceTest
{
    private Mock<IConfigurationHelperCommonService> _configurationHelperServiceMock = null!;
    private Mock<IProjectMemberRepository> _projectMemberRepositoryMock = null!;

    private IProjectMemberFeaturesService _projectMemberFeaturesService = null!;

    [SetUp]
    public void SetUp()
    {
        _configurationHelperServiceMock = new Mock<IConfigurationHelperCommonService>();
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

        _configurationHelperServiceMock
            .Setup(x => x.GetProjectMembersLimitPerProject())
            .Returns(limit);

        _projectMemberFeaturesService = new ProjectMemberFeaturesService
            (_configurationHelperServiceMock.Object, _projectMemberRepositoryMock.Object);
    }
}
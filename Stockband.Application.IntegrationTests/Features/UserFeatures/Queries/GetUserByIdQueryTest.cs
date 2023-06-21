using FizzWare.NBuilder;
using NUnit.Framework;
using Shouldly;
using Stockband.Application.Features.UserFeatures.Queries.GetUserById;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Application.IntegrationTests.Features.UserFeatures.Queries;

public class GetUserByIdQueryTest:BaseTest
{
    private IUserRepository _userRepository = null!;
    [SetUp]
    public void SetUp()
    {
        _userRepository = new UserRepository(Context);
    }
    
    [Test]
    public async Task GetUserByIdQuery_ResponseShouldBeSuccess()
    {
        //Arrange
        const int testingId = 10032;
        
        GetUserByIdQuery query = new GetUserByIdQuery
        {
            Id = testingId
        };

        User userForTest = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingId)
            .With(x => x.Role = UserRole.User)
            .Build();
        await _userRepository.AddAsync(userForTest);

        GetUserByIdQueryHandler handler = new GetUserByIdQueryHandler(_userRepository);
        
        //Act
        BaseResponse<GetUserByIdQueryViewModel> response = await handler.Handle(query, CancellationToken.None);

        //Assert
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        response.Result.ShouldNotBeNull();
    }
    
    [Test]
    public async Task GetUserByIdQuery_InvalidId_ResponseShouldBeNotSuccess()
    {
        //Arrange
        const int testingId = 502;
        
        GetUserByIdQuery query = new GetUserByIdQuery
        {
            Id = testingId
        };

        GetUserByIdQueryHandler handler = new GetUserByIdQueryHandler(_userRepository);
        
        //Act
        BaseResponse<GetUserByIdQueryViewModel> response = await handler.Handle(query, CancellationToken.None);

        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Result.ShouldBeNull();
    }
}
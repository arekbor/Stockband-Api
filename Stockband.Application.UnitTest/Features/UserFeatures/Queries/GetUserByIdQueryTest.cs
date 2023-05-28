using Moq;
using Shouldly;
using Stockband.Application.Features.UserFeatures.Queries.GetUserById;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.UnitTest.RepositoryMocks;
using Stockband.Domain;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;
using Xunit;

namespace Stockband.Application.UnitTest.Features.UserFeatures.Queries;

public class GetUserByIdQueryTest
{
    private readonly Mock<IUserRepository> _mock;

    public GetUserByIdQueryTest()
    {
        _mock = UserRepositoryMock.GetUserRepositoryMock();
    }

    [Fact]
    public async Task GetUserByIdQuery_ShouldBeSuccess()
    {
        int testingId = 1;
        
        GetUserByIdQueryHandler handler = new GetUserByIdQueryHandler(_mock.Object);
        GetUserByIdQuery query = new GetUserByIdQuery
        {
            Id = testingId
        };

        BaseResponse<GetUserByIdQueryViewModel> response = await handler.Handle(query, CancellationToken.None);

        User? userMock = _mock.Object.GetByIdAsync(testingId).Result;

        if (userMock == null)
        {
            throw new ObjectNotFound(typeof(User), testingId);
        }
        
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        response.Result.Id.ShouldBe(userMock.Id);
        response.Result.Email.ShouldBe(userMock.Email);
        response.Result.Username.ShouldBe(userMock.Username);
        response.Result.Role.ShouldBe(userMock.Role);
    }

    [Fact]
    public async Task GetUserByIdQuery_InvalidId_ShouldNotBeSuccess()
    {
        int testingId = 500;
        
        GetUserByIdQueryHandler handler = new GetUserByIdQueryHandler(_mock.Object);
        GetUserByIdQuery query = new GetUserByIdQuery
        {
            Id = testingId
        };
        
        BaseResponse<GetUserByIdQueryViewModel> response = await handler.Handle(query, CancellationToken.None);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Result.ShouldBeNull();
    }
}
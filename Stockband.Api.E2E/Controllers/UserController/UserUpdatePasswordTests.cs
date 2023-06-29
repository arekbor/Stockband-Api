using Stockband.Application.Interfaces.Repositories;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Api.E2E.Controllers.UserController;

public class UserUpdatePasswordTests:BaseTest
{
    private IUserRepository _userRepository = null!;
    private const string TestingUri = "/user/password";
    
    [SetUp]
    public void SetUp()
    {
        _userRepository = new UserRepository(Context);
    }

    //TODO: prepare this
    /*[Test]
    public async Task UserUpdatePassword_BaseResponse_WithoutErrors()
    {
        const int testingUserId = 500;
        string testingEmail = new Faker().Person.Email;
        string testingUsername = new Faker().Person.UserName;
        const string testingCurrentPassword = "TestPassword!@2334";

        string hash = BCrypt.Net.BCrypt.HashPassword(testingCurrentPassword);
        
        User user = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Username = testingUsername)
            .With(x => x.Email = testingEmail)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Password = hash)
            .With(x => x.Id = testingUserId)
            .Build();
        await _userRepository.AddAsync(user);

        UpdateUserPasswordDto dto = new UpdateUserPasswordDto
        {
            CurrentPassword = testingCurrentPassword,
            NewPassword = "TestNewPassword@@!1233",
            ConfirmNewPassword = "TestNewPassword@@!1233"
            
        };

        string token = GetJwtToken(testingUserId, testingUsername, testingEmail);

        HttpResponseModule response = HttpHost
            .Put
            .Url(TestingUri)
            .WithJwtToken(token)
            .Json(dto)
            .Send()
            .Response;

        response.AssertStatusCode(HttpStatusCode.OK);
    }*/
}
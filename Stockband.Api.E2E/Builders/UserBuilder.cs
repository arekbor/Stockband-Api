using FizzWare.NBuilder;
using Stockband.Application.FeatureServices;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Infrastructure;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Api.E2E.Builders;

internal class UserBuilder:BaseTest
{
    private readonly IUserRepository _userRepository;
    private readonly IUserFeaturesService _userFeaturesService;
    
    internal UserBuilder(StockbandDbContext context)
    {
        _userRepository = new UserRepository(context);
        _userFeaturesService = new UserFeaturesService(_userRepository);
    }
    
    internal async Task Build
        (int userId, string? username = "", string? email = "", string? password = "" ,UserRole userRole = UserRole.User)
    {
        User user = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Role = userRole)
            .With(x => x.Password = HashPassword(password))
            .With(x => x.Email = email ?? "test@gmail.com")
            .With(x => x.Username = username ?? "testUsername")
            .With(x => x.Id = userId)
            .Build();
        await _userRepository.AddAsync(user);
    }

    private string HashPassword(string? password)
    {
        if (String.IsNullOrEmpty(password))
        {
            return _userFeaturesService.HashPassword("testTest@@##12345");
        }
        return _userFeaturesService.HashPassword(password);
    }
}
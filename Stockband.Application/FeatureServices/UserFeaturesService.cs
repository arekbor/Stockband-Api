using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.Repositories;

namespace Stockband.Application.FeatureServices;

public class UserFeaturesService:IUserFeaturesService
{
    private readonly IUserRepository _userRepository;

    public UserFeaturesService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<bool> IsEmailAlreadyUsed(string email) =>
        await _userRepository.GetUserByEmailAsync(email) != null;

    public string HashPassword(string password) => 
        BCrypt.Net.BCrypt.HashPassword(password);

    public bool VerifyHashedPassword(string password, string hashedPassword) =>
        BCrypt.Net.BCrypt.Verify(password, hashedPassword);

    public async Task<bool> IsUserExists(int userId) =>
        await _userRepository.GetByIdAsync(userId) != null;
}
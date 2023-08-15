using Stockband.Domain.Entities;

namespace Stockband.Application.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<bool> IsUserRefreshTokenIsUnique(string token);
    Task<User?> GetUserByRefreshToken(string token);
}
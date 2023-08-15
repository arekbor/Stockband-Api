using Microsoft.EntityFrameworkCore;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Entities;

namespace Stockband.Infrastructure.Repositories;

public class UserRepository: BaseRepository<User>, IUserRepository
{
    public UserRepository(StockbandDbContext stockbandDbContext) : base(stockbandDbContext)
    {
    }
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _stockbandDbContext
            .Users
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _stockbandDbContext
            .Users
            .FirstOrDefaultAsync(x => x.Username == username);
    }

    public async Task<bool> IsUserRefreshTokenIsUnique(string token)
    {
        return await _stockbandDbContext
            .Users
            .AnyAsync(x => x.UserRefreshTokens.Any(y => y.Token == token));
    }

    public async Task<User?> GetUserByRefreshToken(string token)
    {
        return await _stockbandDbContext
            .Users
            .Include(x => x.UserRefreshTokens)
            .FirstOrDefaultAsync(x => x.UserRefreshTokens.Any(t => t.Token == token));
    }
}
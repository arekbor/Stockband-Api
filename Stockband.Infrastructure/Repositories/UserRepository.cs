using Microsoft.EntityFrameworkCore;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Entities;
using Stockband.Infrastructure.Configuration;

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
}
using System.Collections;
using Microsoft.EntityFrameworkCore;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Entities;

namespace Stockband.Infrastructure.Repositories;

public class RefreshTokenRepository:BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(StockbandDbContext stockbandDbContext) : base(stockbandDbContext)
    {
    }

    public async Task<RefreshToken?> GetRefreshTokenByToken(string token)
    {
        return await _stockbandDbContext
            .RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == token);
    }
    
    public async Task<RefreshToken?> GetRefreshTokenByReplacedByToken(string replacedByToken)
    {
        return await _stockbandDbContext
            .RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == replacedByToken);
    }

    public async Task<List<RefreshToken>> GetAllUserRefreshTokens(int userId)
    {
        return await _stockbandDbContext
            .RefreshTokens
            .Where(x => x.UserId == userId).ToListAsync();
    }
}
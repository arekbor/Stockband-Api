using Stockband.Domain.Entities;

namespace Stockband.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository:IBaseRepository<RefreshToken>
{ 
    Task<RefreshToken?> GetRefreshTokenByToken(string token);
    Task<RefreshToken?> GetRefreshTokenByReplacedByToken(string replacedByToken);
    Task<List<RefreshToken>> GetAllUserRefreshTokens(int userId);
}
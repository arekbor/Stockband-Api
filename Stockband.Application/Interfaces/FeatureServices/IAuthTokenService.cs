using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Models;

namespace Stockband.Application.Interfaces.FeatureServices;

public interface IAuthTokenService
{
   Task<BaseResponse<AuthTokenResult>> GetTokens(User user);
   Task<BaseResponse<AuthTokenResult>> RefreshToken(string token);
   Task<BaseResponse> RevokeToken(string token);
}
using MediatR;
using Stockband.Domain.Common;

namespace Stockband.Application.Features.UserFeatures.Queries.RefreshToken;

public class RefreshTokenQuery:IRequest<BaseResponse<RefreshTokenQueryViewModel>>
{
    public bool Cookie { get; set; } = true;
    public string Token { get; set; }
}
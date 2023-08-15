using MediatR;
using Stockband.Domain.Common;

namespace Stockband.Application.Features.UserFeatures.Queries.RefreshToken;

public class RefreshTokenQuery:IRequest<BaseResponse<RefreshTokenQueryViewModel>>
{
    public string RefreshToken { get; set; }
}
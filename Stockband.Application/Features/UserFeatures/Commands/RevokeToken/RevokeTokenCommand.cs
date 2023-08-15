using MediatR;
using Stockband.Domain.Common;

namespace Stockband.Application.Features.UserFeatures.Commands.RevokeToken;

public class RevokeTokenCommand:IRequest<BaseResponse>
{
    public string RefreshToken { get; set; }
}
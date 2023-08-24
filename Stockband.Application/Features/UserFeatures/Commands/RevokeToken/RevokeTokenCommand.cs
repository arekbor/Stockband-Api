using MediatR;
using Stockband.Domain.Common;

namespace Stockband.Application.Features.UserFeatures.Commands.RevokeToken;

public class RevokeTokenCommand:IRequest<BaseResponse>
{
    public bool Cookie { get; set; } = true;
    public string RefreshToken { get; set; }
}
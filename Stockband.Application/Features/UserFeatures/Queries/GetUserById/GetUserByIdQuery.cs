using MediatR;
using Stockband.Domain.Common;

namespace Stockband.Application.Features.UserFeatures.Queries.GetUserById;


public class GetUserByIdQuery:IRequest<BaseResponse<GetUserByIdQueryViewModel>>
{
    public int Id { get; set; }
}
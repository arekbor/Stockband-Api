using MediatR;
using Stockband.Application.Common.Attributes;
using Stockband.Domain;
using Stockband.Domain.Common;

namespace Stockband.Application.Features.UserFeatures.Queries.GetUserById;

[AllowRole(UserRole.Admin)]
public class GetUserByIdQuery:IRequest<BaseResponse<GetUserByIdQueryViewModel>>
{
    public int Id { get; set; }
}
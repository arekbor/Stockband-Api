using MediatR;
using Stockband.Domain;

namespace Stockband.Application.Features.ProjectMemberFeatures.Queries.GetAllProjectMembers;

public class GetAllProjectMembersQuery:IRequest<BaseResponse<List<GetAllProjectMembersQueryViewModel>>>
{
    public int RequestedUserId { get; set; }
    public int ProjectId { get; set; }
}
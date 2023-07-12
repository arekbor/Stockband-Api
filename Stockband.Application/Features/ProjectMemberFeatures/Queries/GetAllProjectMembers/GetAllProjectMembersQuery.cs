using MediatR;
using Stockband.Domain.Common;

namespace Stockband.Application.Features.ProjectMemberFeatures.Queries.GetAllProjectMembers;

public class GetAllProjectMembersQuery:IRequest<BaseResponse<List<GetAllProjectMembersQueryViewModel>>>
{
    public GetAllProjectMembersQuery()
    {
        
    }
    
    public GetAllProjectMembersQuery(int projectId)
    {
        ProjectId = projectId;
    }
    public int ProjectId { get; set; }
}
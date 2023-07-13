using MediatR;
using Stockband.Domain.Common;

namespace Stockband.Application.Features.ProjectFeatures.Queries.GetAllUserProjects;

public class GetAllUserProjectsQuery:IRequest<BaseResponse<List<GetAllUserProjectsQueryViewModel>>>
{
    public GetAllUserProjectsQuery()
    {
        
    }
    
    public GetAllUserProjectsQuery(int userId)
    {
        UserId = userId;
    }
    public int UserId { get; set; }
}
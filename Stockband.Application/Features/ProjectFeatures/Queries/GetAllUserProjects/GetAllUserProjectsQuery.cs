using MediatR;
using Stockband.Domain.Common;

namespace Stockband.Application.Features.ProjectFeatures.Queries.GetAllUserProjects;

public class GetAllUserProjectsQuery:IRequest<BaseResponse<List<GetAllUserProjectsQueryViewModel>>>
{
    public int UserId { get; set; }
}
using MediatR;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;

namespace Stockband.Application.Features.ProjectFeatures.Queries.GetAllUserProjects;

public class GetAllUserProjectsQueryHandler:IRequestHandler<GetAllUserProjectsQuery, BaseResponse<List<GetAllUserProjectsQueryViewModel>>>
{
    private readonly IProjectRepository _projectRepository;

    public GetAllUserProjectsQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<BaseResponse<List<GetAllUserProjectsQueryViewModel>>> Handle(GetAllUserProjectsQuery request, CancellationToken cancellationToken)
    {
        return new BaseResponse<List<GetAllUserProjectsQueryViewModel>>
            (await GetAllUserProjectsQueryViewModels(request.UserId));
    }

    private async Task<List<GetAllUserProjectsQueryViewModel>> GetAllUserProjectsQueryViewModels(int userId)
    {
        IEnumerable<Project> projects = 
            await _projectRepository.GetAllProjectsByOwnerId(userId);
        
        List<GetAllUserProjectsQueryViewModel> getAllUserProjectsQueryViewModels =
            new List<GetAllUserProjectsQueryViewModel>();

        foreach (Project project in projects)
        {
            getAllUserProjectsQueryViewModels.Add(new GetAllUserProjectsQueryViewModel
            {
                ProjectId = project.Id,
                OwnerId = project.OwnerId,
                Name = project.Name,
                Description = project.Description
            });
        }

        return getAllUserProjectsQueryViewModels;
    }
}
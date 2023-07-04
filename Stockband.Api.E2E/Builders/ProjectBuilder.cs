using FizzWare.NBuilder;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Entities;
using Stockband.Infrastructure;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Api.E2E.Builders;

internal class ProjectBuilder:BaseTest
{
    private readonly IProjectRepository _projectRepository;

    internal ProjectBuilder(StockbandDbContext context)
    {
        _projectRepository = new ProjectRepository(context);
    }

    internal async Task Build
        (int projectId, int? ownerProjectId = null, string? projectName = "", string? projectDescription = "")
    {
        Project project = Builder<Project>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = projectId)
            .With(x => x.Name = projectName ?? "test project name")
            .With(x => x.Description = projectDescription ?? "test project description")
            .Build();

        if (ownerProjectId != null)
        {
            project.OwnerId = (int)ownerProjectId;
        }

        await _projectRepository.AddAsync(project);
    }
}
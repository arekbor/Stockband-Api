using FizzWare.NBuilder;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Entities;
using Stockband.Infrastructure.Configuration;
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

    internal async Task BuildMany(int? projectOwnerId = null, int size = 1)
    {
        List<Project> projects = Builder<Project>
            .CreateListOfSize(size)
            .All()
            .With(x => x.Deleted = false)
            .Build()
            .ToList();

        if (projectOwnerId != null)
        {
            foreach (Project project in projects)
            {
                project.OwnerId = (int)projectOwnerId;
            }
        }

        await _projectRepository.AddRangeAsync(projects);
    }
}
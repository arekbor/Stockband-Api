using Moq;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.UnitTest.RepositoryMocks;

public class ProjectRepositoryMock
{
    public static Mock<IProjectRepository> GetProjectRepositoryMock()
    {
        Mock<IUserRepository> userRepository = UserRepositoryMock.GetUserRepositoryMock();
        
        List<Project> projectMocks = GetProjectMocks();

        Mock<IProjectRepository> projectRepositoryMock = new Mock<IProjectRepository>();
        
        
        projectRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(projectMocks);

        projectRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Project>())).Callback<Project>((project) =>
        {
            projectMocks.Add(project);
        });
        
        projectRepositoryMock.Setup(r => r.GetProjectByNameAsync(It.IsAny<string>())).ReturnsAsync((string name) =>
        {
            Project? project = projectMocks
                .FirstOrDefault(x => x.Name == name);
            return project;
        });
        
        projectRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))!.ReturnsAsync((int id) =>
        {
            Project? project = projectMocks
                .FirstOrDefault(x => x.Id == id);
            return project;
        });
        
        projectRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Project>())).Callback<Project>((project) =>
        {
            int index = projectMocks.FindIndex(x => x.Id == project.Id);
            projectMocks[index] = project;
        });
        
        projectRepositoryMock.Setup(r => r.GetProjectByIdWithIncludedUserAsync(It.IsAny<int>())).ReturnsAsync((int id) =>
        {
            Project? project = projectMocks
                .FirstOrDefault(x => x.Id == id);

            if (project != null)
            {
                User? user = userRepository.Object.GetByIdAsync(project.OwnerId).Result;
                if (user == null)
                {
                    throw new ObjectNotFound(typeof(User), project.OwnerId);
                }
                project.Owner = user;
            }
            
            return project;
        });

        return projectRepositoryMock;
    }
    public static List<Project> GetProjectMocks()
    {
        return new List<Project>
        {
            new Project()
            {
                Id = 1,
                OwnerId = 1,
                Name = "Project test 1",
                Description = "Project description 1"
            },
            new Project()
            {
                Id = 2,
                OwnerId = 2,
                Name = "Project test 2",
                Description = "Project description 2"
            },
            new Project()
            {
                Id = 3,
                OwnerId = 2,
                Name = "Project test 3",
                Description = "Project description 3"
            }
        };
    }
}
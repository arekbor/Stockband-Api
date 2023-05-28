using Moq;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Entities;

namespace Stockband.Application.UnitTest.RepositoryMocks;

public class ProjectMemberRepositoryMock
{
    public static Mock<IProjectMemberRepository> GetProjectMemberRepositoryMock()
    {
        Mock<IProjectRepository> projectRepositoryMock = ProjectRepositoryMock.GetProjectRepositoryMock();
        Mock<IUserRepository> userRepositoryMock = UserRepositoryMock.GetUserRepositoryMock();
        
        
        List<ProjectMember> projectMemberMocks = GetProjectMemberMocks();
        
        Mock<IProjectMemberRepository> projectMemberRepositoryMock = new Mock<IProjectMemberRepository>();

        projectMemberRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(projectMemberMocks);
        
        projectMemberRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ProjectMember>())).Callback<ProjectMember>((projectMember) =>
        {
            projectMemberMocks.Add(projectMember);
        });
        
        projectMemberRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<ProjectMember>())).Callback<ProjectMember>((projectMember) =>
        {
            projectMemberMocks.Remove(projectMember);
        });

        projectMemberRepositoryMock.Setup(r => r.GetAllProjectMembersByProjectIdAsync(It.IsAny<int>()))!.ReturnsAsync((int projectId) =>
        {
            IEnumerable<ProjectMember> projectMembers = projectMemberMocks
                .Where(x => x.ProjectId == projectId)
                .ToList();
            return projectMembers;
        });

        projectMemberRepositoryMock.Setup(r => r.GetProjectMemberIncludedProjectAndMemberAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((int projectId, int memberId) =>
        {
            ProjectMember? projectMember = projectMemberMocks
                .FirstOrDefault(x => x.ProjectId == projectId && x.MemberId == memberId);

            if (projectMember != null)
            {
                Project? project = projectRepositoryMock.Object.GetByIdAsync(projectId).Result;
                if (project != null)
                {
                    projectMember.Project = project;
                }
                
                User? member = userRepositoryMock.Object.GetByIdAsync(memberId).Result;
                if (member != null)
                {
                    projectMember.Member = member;
                }
            }

            return projectMember;
        });
        
        return projectMemberRepositoryMock;
    }

    public static List<ProjectMember> GetProjectMemberMocks()
    {
        return new List<ProjectMember>
        {
            new ProjectMember
            {
                Id = 1,
                MemberId = 1,
                ProjectId = 1
            },
            new ProjectMember
            {
                Id = 2,
                MemberId = 2,
                ProjectId = 1
            },
            new ProjectMember
            {
                Id = 3,
                MemberId = 3,
                ProjectId = 1
            },
            new ProjectMember
            {
                Id = 4,
                MemberId = 3,
                ProjectId = 4
            },
            new ProjectMember
            {
                Id = 5,
                MemberId = 3,
                ProjectId = 5
            },
            new ProjectMember
            {
                Id = 6,
                MemberId = 3,
                ProjectId = 6
            },
        };
    }
}
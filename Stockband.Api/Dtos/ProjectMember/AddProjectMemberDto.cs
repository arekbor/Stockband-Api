namespace Stockband.Api.Dtos.ProjectMember;

public class AddProjectMemberDto
{
    public AddProjectMemberDto()
    {
        
    }

    public AddProjectMemberDto(int projectId, int memberId)
    {
        ProjectId = projectId;
        MemberId = memberId;
    }
    
    public int ProjectId { get; set; }
    public int MemberId { get; set; }
}
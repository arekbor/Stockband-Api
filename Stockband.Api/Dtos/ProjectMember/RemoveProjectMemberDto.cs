namespace Stockband.Api.Dtos.ProjectMember;

public class RemoveProjectMemberDto
{
    public RemoveProjectMemberDto()
    {
        
    }

    public RemoveProjectMemberDto(int projectId, int memberId)
    {
        ProjectId = projectId;
        MemberId = memberId;
    }
    
    public int ProjectId { get; set; }
    public int MemberId { get; set; }
}
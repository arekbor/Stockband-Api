using Stockband.Domain.Common;

namespace Stockband.Api.Dtos.User;

public class UpdateRoleDto
{
    public UpdateRoleDto()
    {
        
    }
    public UpdateRoleDto(int userId, UserRole userRole)
    {
        UserId = userId;
        Role = userRole;
    }
    public int UserId { get; set; }
    public UserRole Role { get; set; }
}
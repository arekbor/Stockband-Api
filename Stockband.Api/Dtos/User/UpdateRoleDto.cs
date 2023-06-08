using Stockband.Domain.Common;

namespace Stockband.Api.Dtos.User;

public class UpdateRoleDto
{
    public int UserId { get; set; }
    public UserRole Role { get; set; }
}
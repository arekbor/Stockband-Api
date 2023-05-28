using Stockband.Domain.Common;

namespace Stockband.Application.Features.UserFeatures.Queries.GetUserById;

public class GetUserByIdQueryViewModel
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }
}
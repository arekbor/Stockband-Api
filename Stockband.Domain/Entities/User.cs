using Stockband.Domain.Common;
using Stockband.Domain.Enums;

namespace Stockband.Domain.Entities;

public class User : AuditEntity
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }
    public List<UserRefreshToken> UserRefreshTokens { get; set; } = new List<UserRefreshToken>();
}
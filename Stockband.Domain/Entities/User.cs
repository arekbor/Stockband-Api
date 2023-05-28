using Stockband.Domain.Common;

namespace Stockband.Domain.Entities;

public class User : AuditEntity
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }
}
using Stockband.Domain.Common;
using Stockband.Domain.Enums;

namespace Stockband.Domain.Entities;

public class RefreshToken:AuditEntity
{
    public int UserId { get; set; }
    public virtual User User { get; set; }
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public string CreatedByIp { get; set; }
    public DateTime? Revoked { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public RefreshTokenReasonRevoke? ReasonRevoke { get; set; }
    public virtual bool IsExpired => DateTime.Now >= Expires;
    public virtual bool IsRevoked => Revoked != null;
    public virtual bool IsActive => !IsRevoked && !IsExpired;
}
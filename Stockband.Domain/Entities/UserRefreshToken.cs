using Stockband.Domain.Common;
using Stockband.Domain.Enums;

namespace Stockband.Domain.Entities;

public class UserRefreshToken:AuditEntity
{
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public string CreatedByIp { get; set; }
    public DateTime? Revoked { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public RefreshTokenReasonRevoke? ReasonRevoke { get; set; }
    public virtual bool IsExpired =>
        DateTime.Now >= Expires;
    public virtual bool IsRevoked => 
        Revoked != null;
    public bool IsActive => 
        !IsRevoked && !IsExpired;
}
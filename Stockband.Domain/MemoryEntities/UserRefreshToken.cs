using Stockband.Domain.Common;
using Stockband.Domain.Enums;

namespace Stockband.Domain.MemoryEntities;

public class UserRefreshToken:MemoryAuditEntity
{
    public string Token { get; set; }
    public int UserId { get; set; }
    public DateTime Expiration { get; set; }
    public DateTime? Revoke { get; set; }
    public string UserIp { get; set; }
    public UserRefreshTokenRevokeReason RevokeReason { get; set; }
    public virtual bool IsExpired =>
        DateTime.UtcNow >= Expiration;
    public virtual bool IsRevoked =>
        Revoke != null;
}
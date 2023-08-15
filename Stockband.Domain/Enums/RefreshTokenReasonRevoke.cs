namespace Stockband.Domain.Enums;

public enum RefreshTokenReasonRevoke
{
    AttemptedReuseRevokedToken,
    ReplacedByNewToken,
    ManualRevoked,
}
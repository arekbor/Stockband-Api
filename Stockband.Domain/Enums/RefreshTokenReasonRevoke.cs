namespace Stockband.Domain.Enums;

public enum RefreshTokenReasonRevoke
{
    Manual,
    AttemptedReuseRevokedToken,
    ReplacedByNewToken
}
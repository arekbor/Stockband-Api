namespace Stockband.Domain.Common;

public class AuthorizationTokensResponse
{
    public string Token { get; set; }
    public string? RefreshToken { get; set; }
}
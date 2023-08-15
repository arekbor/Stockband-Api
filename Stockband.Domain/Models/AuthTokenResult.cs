using Stockband.Domain.Entities;

namespace Stockband.Domain.Models;

public class AuthTokenResult
{
    public string AccessToken { get; set; }
    public UserRefreshToken UserRefreshToken { get; set; }
}
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Stockband.Application.Interfaces.ExternalServices;
using Stockband.Domain.Enums;
using Stockband.Domain.Exceptions;

namespace Stockband.Infrastructure.Services;

public class AuthenticationUserService:IAuthenticationUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfigurationHelperService _configurationHelperService;
    
    public AuthenticationUserService(
        IHttpContextAccessor httpContextAccessor,
        IConfigurationHelperService configurationHelperService)
    {
        _httpContextAccessor = httpContextAccessor;
        _configurationHelperService = configurationHelperService;
    }
    
    /// <summary>
    /// Generates a JWT token based on the provided user information.
    /// </summary>
    /// <param name="userId">The user ID associated with the token.</param>
    /// <param name="username">The username associated with the token.</param>
    /// <param name="email">The email associated with the token.</param>
    /// <param name="role">The role associated with the token.</param>
    /// <returns>The generated JWT token.</returns>
    public string GetAccessToken(int userId, string username, string email, UserRole role)
    {
        JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        string jwtKey = _configurationHelperService.GetAccessTokenPrivateKey();

        string jwtAudience = _configurationHelperService.GetAccessTokenAudience();

        string jwtIssuer = _configurationHelperService.GetAccessTokenIssuer();

        double jwtExpires = _configurationHelperService.GetAccessTokenExpiresInMinutes();
        
        byte[] keyBytes = Encoding.UTF8.GetBytes(jwtKey);
        SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new (ClaimTypes.NameIdentifier, userId.ToString()),
                new (ClaimTypes.Name, username),
                new (ClaimTypes.Email, email),
                new (ClaimTypes.Role, role.ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(jwtExpires),
            Audience = jwtAudience,
            Issuer = jwtIssuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
        };
        
        SecurityToken token = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
        string tokenString = jwtSecurityTokenHandler.WriteToken(token);

        return tokenString;
    }

    /// <summary>
    /// Sets a value as an HttpOnly cookie.
    /// </summary>
    /// <param name="cookieName">The name of the cookie that will be created.</param>
    /// <param name="value">The value to be stored in the cookie.</param>
    /// <param name="cookieExpires">The expiration for the cookie.</param>
    public void SetValueAsHttpOnlyCookie(string cookieName, string value, DateTimeOffset cookieExpires)
    {
        HttpContext context = GetHttpContext();
        
        CookieOptions cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = cookieExpires
        };
        
        context.Response.Cookies.Append(cookieName, value, cookieOptions);
    }

    /// <summary>
    /// Retrieves a value from a specific cookie.
    /// </summary>
    /// <param name="cookieName">The name of the cookie.</param>
    /// <returns>The value stored in the specific cookie.</returns>
    public string? GetValueFromCookie(string cookieName)
    {
        HttpContext context = GetHttpContext();
        return context.Request.Cookies[cookieName];
    }

    /// <summary>
    /// Invalidates a cookie by setting its expiration time to a date in the past.
    /// </summary>
    /// <param name="cookieName">The name of the cookie.</param>
    public void InvalidateCookie(string cookieName)
    {
        HttpContext context = GetHttpContext();
        
        CookieOptions cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(-1)
        };
        context.Response.Cookies.Append(cookieName, String.Empty, cookieOptions);
    }

    public int GetUserId()
    {
        Claim? claim = GetHttpContext().User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        if (claim == null)
        {
            throw new ObjectNotFound(typeof(Claim));
        }
        
        bool success = int.TryParse(claim.Value, out int parsedId);
        if (!success)
        {
            throw new FormatException();
        }

        return parsedId;
    }
    
    /// <summary>
    /// Verifies whether the provided user ID belongs to an admin or is equal to the claim ID.
    /// </summary>
    /// <param name="userId">The user ID to be verified.</param>
    /// <returns><c>true</c> if the user is authorized; otherwise, <c>false</c>.</returns>
    public bool IsAuthorized(int userId) =>
        GetUserId() == userId || GetHttpContext().User.IsInRole(UserRole.Admin.ToString());

    public string GetUserIp()
    {
        HttpContext httpContext = GetHttpContext();
        const string headerName = "X-Forwarded-For";
        
        bool containsKey = httpContext.Request.Headers.ContainsKey(headerName);
        if (containsKey)
        {
            StringValues values = httpContext.Request.Headers[headerName];
            return values.ToString();
        }

        IPAddress? ipAddress = httpContext.Connection.RemoteIpAddress;
        if (ipAddress == null)
        {
            throw new ObjectNotFound(typeof(IPAddress));
        }
        return ipAddress.MapToIPv4().ToString();
    }

    private HttpContext GetHttpContext()
    {
        HttpContext? httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new ObjectNotFound(typeof(HttpContext));
        }
        return httpContext;
    }
}
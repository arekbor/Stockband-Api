using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Stockband.Application.Interfaces.Services;
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
    
    public string GenerateJwtToken(string userId, string username, string email, string role)
    {
        JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        string jwtKey = _configurationHelperService.GetJwtKey();

        string jwtAudience = _configurationHelperService.GetJwtAudience();

        string jwtIssuer = _configurationHelperService.GetJwtIssuer();

        double jwtExpires = _configurationHelperService.GetJwtExpires();
        
        byte[] keyBytes = Encoding.UTF8.GetBytes(jwtKey);
        SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new (ClaimTypes.NameIdentifier, userId),
                new (ClaimTypes.Name, username),
                new (ClaimTypes.Email, email),
                new (ClaimTypes.Role, role)
            }),
            Expires = DateTime.Now.AddMinutes(jwtExpires),
            Audience = jwtAudience,
            Issuer = jwtIssuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
        };
        
        SecurityToken token = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
        string tokenString = jwtSecurityTokenHandler.WriteToken(token);

        return tokenString;
    }

    public void AddJwtCookie(string jwtToken)
    {
        string cookieName = _configurationHelperService.GetCookieName();

        double cookieExpires = _configurationHelperService.GetCookieExpires();
        
        HttpContext? httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new ObjectNotFound(typeof(HttpContext));
        }
        
        httpContext.Response.Cookies.Append(cookieName, jwtToken, new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.Now.AddMinutes(cookieExpires),
            SameSite = SameSiteMode.None
        });
    }

    public void ClearJwtCookie()
    {
        string cookieName = _configurationHelperService.GetCookieName();

        HttpContext? httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new ObjectNotFound(typeof(HttpContext));
        }
        
        httpContext.Response.Cookies.Delete(cookieName);
    }

    public int GetCurrentUserId()
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

    public IEnumerable<string> GetCurrentUserRoles()
    {
        return GetHttpContext()
            .User
            .Claims
            .Where(x => x.Type == ClaimTypes.Role)
            .Select(x => x.Value);
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
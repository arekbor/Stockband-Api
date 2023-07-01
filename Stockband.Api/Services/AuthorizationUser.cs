using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Stockband.Api.Interfaces;
using Stockband.Application.Interfaces.Common;
using Stockband.Domain.Exceptions;

namespace Stockband.Api.Services;

public class AuthorizationUser:IAuthorizationUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfigurationHelperCommonService _configurationHelperCommonService;

    public AuthorizationUser(
        IHttpContextAccessor httpContextAccessor, 
        IConfigurationHelperCommonService configurationHelperCommonService)
    {
        _httpContextAccessor = httpContextAccessor;
        _configurationHelperCommonService = configurationHelperCommonService;
    }
    public string CreateJwt(string userId, string username, string email, string role)
    {
        JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        string jwtKey = _configurationHelperCommonService.GetJwtKey();

        string jwtAudience = _configurationHelperCommonService.GetJwtAudience();

        string jwtIssuer = _configurationHelperCommonService.GetJwtIssuer();

        double jwtExpires = _configurationHelperCommonService.GetJwtExpires();
        
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
        string cookieName = _configurationHelperCommonService.GetCookieName();

        double cookieExpires = _configurationHelperCommonService.GetCookieExpires();
        
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
        string cookieName = _configurationHelperCommonService.GetCookieName();

        HttpContext? httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new ObjectNotFound(typeof(HttpContext));
        }
        
        httpContext.Response.Cookies.Delete(cookieName);
    }

    public int GetUserIdFromClaims()
    {
        HttpContext? httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new ObjectNotFound(typeof(HttpContext));
        }
        
        Claim? claim = httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
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
}
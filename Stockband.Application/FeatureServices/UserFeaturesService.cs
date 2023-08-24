using Stockband.Application.Interfaces.ExternalServices;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Enums;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.FeatureServices;

public class UserFeaturesService:IUserFeaturesService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationUserService _authenticationUserService;
    private readonly IConfigurationHelperService _configurationHelperService;
    public UserFeaturesService(IUserRepository userRepository, 
        IAuthenticationUserService authenticationUserService, 
        IConfigurationHelperService configurationHelperService)
    {
        _userRepository = userRepository;
        _authenticationUserService = authenticationUserService;
        _configurationHelperService = configurationHelperService;
    }
    
    public async Task<bool> IsEmailAlreadyUsed(string email) =>
        await _userRepository.GetUserByEmailAsync(email) != null;

    public string HashPassword(string password) => 
        BCrypt.Net.BCrypt.HashPassword(password);

    public bool VerifyHashedPassword(string password, string hashedPassword) =>
        BCrypt.Net.BCrypt.Verify(password, hashedPassword);

    public async Task<bool> IsUserExists(int userId) =>
        await _userRepository.GetByIdAsync(userId) != null;
    
    /// <summary>
    /// Generates a tokens response with the provided access token and optional refresh token,
    /// based on the specified cookie flag.
    /// </summary>
    /// <param name="accessToken">The access token to include in the response.</param>
    /// <param name="refreshToken">The refresh token to include in the response.</param>
    /// <param name="cookie">Flag indicating whether to store the refresh token in a cookie.</param>
    /// <returns>
    /// A tokens response containing the access token and, if the cookie flag is set to true,
    /// the refresh token stored in a cookie; otherwise, the refresh token is included in the response object.
    /// </returns>
    public AuthorizationTokensResponse ComposeAuthorizationTokens(string accessToken, string refreshToken, bool cookie)
    {
        AuthorizationTokensResponse response = new AuthorizationTokensResponse
        {
            Token = accessToken
        };

        if (cookie)
        {
            string refreshTokenCookieName = _configurationHelperService.GetRefreshTokenCookieName();
            double refreshTokenCookieExpiresInDays = _configurationHelperService.GetRefreshTokenCookieExpiresInDays();
            _authenticationUserService.SetValueAsHttpOnlyCookie
            (
                cookieName: refreshTokenCookieName, 
                value: refreshToken, 
                cookieExpires: DateTime.Now.AddDays(refreshTokenCookieExpiresInDays)
            );
        }
        else
        {
            response.RefreshToken = refreshToken;
        }

        return response;
    }
    
    /// <summary>
    /// Retrieves the refresh token from the specified source, either a cookie or a variable.
    /// </summary>
    /// <param name="refreshToken">The refresh token value or identifier.</param>
    /// <param name="cookie">Flag indicating whether to read from a cookie as the source.
    /// If true, the value of refreshToken parameter is not required</param>
    /// <returns>A response containing the retrieved refresh token</returns>
    public BaseResponse<string> GetRefreshTokenFromSource(string refreshToken, bool cookie)
    {
        string token;

        if (cookie)
        {
            string refreshTokenCookieName = _configurationHelperService.GetRefreshTokenCookieName();
            string? refreshTokenFromCookie = _authenticationUserService.GetValueFromCookie(refreshTokenCookieName);
            if (refreshTokenFromCookie == null)
            {
                return new BaseResponse<string>(new ObjectNotFound(typeof(RefreshToken)), 
                    BaseErrorCode.CookieRefreshTokenNotFound);
            }

            token = refreshTokenFromCookie;
        }
        else
        {
            token = refreshToken;
        }

        return new BaseResponse<string>(token);
    }
}
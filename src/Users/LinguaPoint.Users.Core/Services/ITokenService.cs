using LinguaPoint.Users.Core.Models;

namespace LinguaPoint.Users.Core.Services;

public interface ITokenService
{
    /// <summary>
    /// Generates a short-lived access token for the user
    /// </summary>
    /// <param name="user">The user for whom to generate the token</param>
    /// <returns>A JWT access token string</returns>
    string GenerateAccessToken(User user);
    
    /// <summary>
    /// Generates a longer-lived refresh token for the user
    /// </summary>
    /// <param name="user">The user for whom to generate the token</param>
    /// <returns>A JWT refresh token string</returns>
    string GenerateRefreshToken(User user);
    
    /// <summary>
    /// Validates an access token and returns the user ID if valid
    /// </summary>
    /// <param name="token">The access token to validate</param>
    /// <returns>The user ID if the token is valid, null otherwise</returns>
    Guid? ValidateAccessToken(string token);
    
    /// <summary>
    /// Validates a refresh token and returns the user ID if valid
    /// </summary>
    /// <param name="token">The refresh token to validate</param>
    /// <returns>The user ID if the token is valid, null otherwise</returns>
    Guid? ValidateRefreshToken(string token);
}
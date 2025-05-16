using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LinguaPoint.Users.Core.Models;
using LinguaPoint.Users.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LinguaPoint.Users.Infrastructure.Services;

public class JwtTokenService : ITokenService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly SymmetricSecurityKey _accessTokenKey;
    private readonly SymmetricSecurityKey _refreshTokenKey;

    public JwtTokenService(IConfiguration configuration)
    {
        _issuer = configuration["Jwt:Issuer"] ?? "LinguaPoint";
        _audience = configuration["Jwt:Audience"] ?? "LinguaPointUsers";
        
        // In a real-world scenario, these keys should be stored securely
        // and loaded from configuration or environment variables
        var accessTokenSecret = configuration["Jwt:AccessTokenSecret"] ?? 
                             GenerateRandomSecret();
        var refreshTokenSecret = configuration["Jwt:RefreshTokenSecret"] ?? 
                              GenerateRandomSecret();
        
        _accessTokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessTokenSecret));
        _refreshTokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshTokenSecret));
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30), // Short-lived token
            signingCredentials: new SigningCredentials(_accessTokenKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7), // Longer-lived token
            signingCredentials: new SigningCredentials(_refreshTokenKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Guid? ValidateAccessToken(string token)
    {
        return ValidateToken(token, _accessTokenKey);
    }

    public Guid? ValidateRefreshToken(string token)
    {
        return ValidateToken(token, _refreshTokenKey);
    }

    private Guid? ValidateToken(string token, SecurityKey key)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub);
            
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
        }
        catch
        {
            // Token validation failed
            return null;
        }

        return null;
    }

    private string GenerateRandomSecret()
    {
        // Generate a cryptographically strong random key
        var key = new byte[32]; // 256 bits
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(key);
        return Convert.ToBase64String(key);
    }
}
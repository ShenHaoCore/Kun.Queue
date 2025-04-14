using Kun.Queue.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Kun.Queue.Commons;

/// <summary>
/// 
/// </summary>
public class JwtHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public static (string accessToken, string refreshToken) GenerateToken(string userId, JwtOptions options)
    {
        List<Claim> claims = [new Claim(JwtRegisteredClaimNames.Sub, userId), new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())];
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        JwtSecurityToken token = new JwtSecurityToken(issuer: options.Issuer, audience: options.Audience, claims: claims, expires: DateTime.Now.AddMinutes(15), signingCredentials: creds);
        string refreshToken = GenerateRefreshToken();
        return (new JwtSecurityTokenHandler().WriteToken(token), refreshToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private static string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}

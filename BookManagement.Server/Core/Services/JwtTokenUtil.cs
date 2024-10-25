using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BookManagement.Server.Core.Models;
using BookManagement.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace BookManagement.Server.Core.Services;
public class JwtTokenUtil
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public JwtTokenUtil(IConfiguration configuration, ApplicationDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!)),
            ValidateLifetime = false // we want to get the claims from an expired token
        };

        var principal = new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        // var jwtSecurityToken = securityToken as JwtSecurityToken;

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }


        return principal;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public RefreshToken GenerateRefreshTokenModel()
    {
        return new RefreshToken
        {
            Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
            Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration.GetSection("Jwt:RefreshTokenExpirationInDays").Value)),

            Created = DateTime.UtcNow
        };
    }

    public string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
      new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Sử dụng "sub" cho Id của người dùng
      new Claim(ClaimTypes.Name, user.UserName ?? ""), // Sử dụng "unique_name" cho UserName
      new Claim(ClaimTypes.Email, user.Email ?? "")
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
          issuer: _configuration["Jwt:Issuer"],
          audience: _configuration["Jwt:Audience"],
          claims: claims,
          expires: DateTime.Now.AddMinutes(30),
          signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public void RevokeRefreshToken(User user, string refreshToken)
    {
        var token = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
        if (token != null)
        {
            _context.RefreshTokens.Remove(token);
            _context.SaveChanges();
        }
    }
}
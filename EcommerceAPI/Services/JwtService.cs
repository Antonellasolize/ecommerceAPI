using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcommerceAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace EcommerceAPI.Services;

public class JwtService(IConfiguration config) : IJwtService
{
    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()) // Admin | Empresa | Cliente
        };

        // Solo para usuarios Empresa agregamos el companyId
        if (user.CompanyId != null)
            claims.Add(new Claim("CompanyId", user.CompanyId.Value.ToString()));

        var keyBytes = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);
        var key = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
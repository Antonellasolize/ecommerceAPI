namespace EcommerceAPI.Services;

using EcommerceAPI.Models;

public interface IJwtService
{
    string GenerateToken(User user);
}
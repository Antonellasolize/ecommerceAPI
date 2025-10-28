using EcommerceAPI.Models;

namespace EcommerceAPI.Dto.Auth;

public class UserInfoDto
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public AppRole Role { get; set; }
    public int? CompanyId { get; set; } // null para Admin/Cliente
}
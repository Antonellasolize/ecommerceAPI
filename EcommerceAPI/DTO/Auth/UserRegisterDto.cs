using System.ComponentModel.DataAnnotations;
using EcommerceAPI.Models;

namespace EcommerceAPI.Dto.Auth;

public class UserRegisterDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required, MinLength(6)]
    public string Password { get; set; } = null!;

    [Required]
    public AppRole Role { get; set; } = AppRole.Cliente;

    // Solo si Role = Empresa
    public int? CompanyId { get; set; }
}
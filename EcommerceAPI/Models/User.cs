using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcommerceAPI.Models;

public enum AppRole { Admin = 0, Empresa = 1, Cliente = 2 }

public class User
{
    [Key]
    public int Id { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required, MinLength(6)]
    public string PasswordHash { get; set; } = null!;

    [Required]
    public AppRole Role { get; set; } = AppRole.Cliente;

    // Solo para usuarios con rol Empresa:
    public int? CompanyId { get; set; }

    [JsonIgnore]
    public Company? Company { get; set; }

    // Historial de compras del Cliente (opcional; útil si querés incluir)
    [JsonIgnore]
    public List<Order> Orders { get; set; } = [];
}
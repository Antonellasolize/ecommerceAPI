using EcommerceAPI.Data;
using EcommerceAPI.Dto.Auth;
using EcommerceAPI.Models;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController(AppDbContext context, IJwtService tokenService) : ControllerBase
{
    private readonly PasswordHasher<User> _hasher = new();

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var exists = await context.Users.AnyAsync(u => u.Email == dto.Email);
        if (exists) return BadRequest("Email already registered");

        // Si Role = "Empresa" y viene CompanyId, lo guardamos; si no, null.
        var user = new User
        {
            Email = dto.Email,
            Role  = dto.Role,
            CompanyId = dto.CompanyId   // puede ser null (Admin/Cliente)
        };

        user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> Login(UserLoginDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null) return Unauthorized("Invalid credentials");

        var vr = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (vr == PasswordVerificationResult.Failed) return Unauthorized("Invalid credentials");

        var token = tokenService.GenerateToken(user);
        return Ok(new TokenDto { Token = token });
    }
}
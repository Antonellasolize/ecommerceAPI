using System.Security.Claims;
using EcommerceAPI.Data;
using EcommerceAPI.Dto.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController(AppDbContext context) : ControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult<UserInfoDto>> GetProfile()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var user = await context.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserInfoDto
            {
                Id = u.Id,
                Email = u.Email,
                Role = u.Role,
                CompanyId = u.CompanyId
            })
            .FirstOrDefaultAsync();

        return user is null ? NotFound() : Ok(user);
    }
}
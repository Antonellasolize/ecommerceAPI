using EcommerceAPI.Data;
using EcommerceAPI.Dto.Company;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class CompaniesController(AppDbContext context) : ControllerBase
{
    // GET: api/Companies
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAll()
    {
        var list = await context.Companies
            .Select(c => new CompanyDto
            {
                Id          = c.Id,
                Name        = c.Name,
                OwnerName   = c.OwnerName,
                NetWorth    = c.NetWorth,
                Description = c.Description,
                IsActive    = c.IsActive
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: api/Companies/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CompanyDto>> GetById(int id)
    {
        var c = await context.Companies
            .Where(x => x.Id == id)
            .Select(x => new CompanyDto
            {
                Id          = x.Id,
                Name        = x.Name,
                OwnerName   = x.OwnerName,
                NetWorth    = x.NetWorth,
                Description = x.Description,
                IsActive    = x.IsActive
            })
            .FirstOrDefaultAsync();

        return c is null ? NotFound() : Ok(c);
    }

    // POST: api/Companies
    [HttpPost]
    public async Task<ActionResult<CompanyDto>> Create(CompanyCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var company = new Company
        {
            Name        = dto.Name,
            OwnerName   = dto.OwnerName,
            NetWorth    = dto.NetWorth,
            Description = dto.Description,
            IsActive    = true
        };

        context.Companies.Add(company);
        await context.SaveChangesAsync();

        var result = new CompanyDto
        {
            Id          = company.Id,
            Name        = company.Name,
            OwnerName   = company.OwnerName,
            NetWorth    = company.NetWorth,
            Description = company.Description,
            IsActive    = company.IsActive
        };

        return CreatedAtAction(nameof(GetById), new { id = company.Id }, result);
    }

    // PUT: api/Companies/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CompanyUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var c = await context.Companies.FindAsync(id);
        if (c is null) return NotFound();

        c.Name        = dto.Name;
        c.OwnerName   = dto.OwnerName;
        c.NetWorth    = dto.NetWorth;
        c.Description = dto.Description;

        await context.SaveChangesAsync();
        return NoContent();
    }

    // PATCH: api/Companies/5/toggle
    [HttpPatch("{id:int}/toggle")]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var c = await context.Companies.FindAsync(id);
        if (c is null) return NotFound();

        c.IsActive = !c.IsActive;
        await context.SaveChangesAsync();

        return Ok(new { c.Id, c.IsActive });
    }
}
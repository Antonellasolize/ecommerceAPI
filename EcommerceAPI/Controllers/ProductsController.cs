using System.Security.Claims;
using EcommerceAPI.Data;
using EcommerceAPI.Dto.Product;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(AppDbContext context) : ControllerBase
{
    private int? GetCompanyIdFromToken()
    {
        var val = User.FindFirst("CompanyId")?.Value;
        return int.TryParse(val, out var cid) ? cid : null;
    }

    // --------- Catálogo público ----------
    [HttpGet("catalog")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductDto>>> Catalog(
        [FromQuery] int? companyId, [FromQuery] string? q,
        [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
    {
        var query = context.Products
            .Where(p => p.IsPublished)
            .AsQueryable();

        if (companyId.HasValue) query = query.Where(p => p.CompanyId == companyId.Value);
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(p => p.Name.Contains(q));
        if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice.Value);
        if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice.Value);

        var list = await query
            .Select(p => new ProductDto
            {
                Id = p.Id, CompanyId = p.CompanyId, Name = p.Name,
                Description = p.Description, Price = p.Price, Stock = p.Stock,
                IsPublished = p.IsPublished, ImageUrl = p.ImageUrl
            }).ToListAsync();

        return Ok(list);
    }

    // --------- Panel Empresa ----------
    [HttpGet("my")]
    [Authorize(Roles = "Empresa")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> MyProducts()
    {
        var companyId = GetCompanyIdFromToken();
        if (companyId is null) return Forbid();

        var list = await context.Products
            .Where(p => p.CompanyId == companyId.Value)
            .Select(p => new ProductDto
            {
                Id = p.Id, CompanyId = p.CompanyId, Name = p.Name,
                Description = p.Description, Price = p.Price, Stock = p.Stock,
                IsPublished = p.IsPublished, ImageUrl = p.ImageUrl
            }).ToListAsync();

        return Ok(list);
    }

    [HttpPost]
    [Authorize(Roles = "Empresa")]
    public async Task<ActionResult<ProductDto>> Create(ProductCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var companyId = GetCompanyIdFromToken();
        if (companyId is null) return Forbid();

        var product = new Product
        {
            CompanyId = companyId.Value,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            ImageUrl = dto.ImageUrl,
            IsPublished = false
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var result = new ProductDto
        {
            Id = product.Id, CompanyId = product.CompanyId, Name = product.Name,
            Description = product.Description, Price = product.Price, Stock = product.Stock,
            IsPublished = product.IsPublished, ImageUrl = product.ImageUrl
        };

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Empresa")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var companyId = GetCompanyIdFromToken();
        if (companyId is null) return Forbid();

        var p = await context.Products
            .Where(x => x.Id == id && x.CompanyId == companyId.Value)
            .Select(x => new ProductDto
            {
                Id = x.Id, CompanyId = x.CompanyId, Name = x.Name,
                Description = x.Description, Price = x.Price, Stock = x.Stock,
                IsPublished = x.IsPublished, ImageUrl = x.ImageUrl
            }).FirstOrDefaultAsync();

        return p is null ? NotFound() : Ok(p);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Empresa")]
    public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
    {
        var companyId = GetCompanyIdFromToken();
        if (companyId is null) return Forbid();

        var p = await context.Products.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == companyId.Value);
        if (p is null) return NotFound();

        p.Name = dto.Name;
        p.Description = dto.Description;
        p.Price = dto.Price;
        p.Stock = dto.Stock;
        p.ImageUrl = dto.ImageUrl;
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id:int}/publish")]
    [Authorize(Roles = "Empresa")]
    public async Task<IActionResult> TogglePublish(int id)
    {
        var companyId = GetCompanyIdFromToken();
        if (companyId is null) return Forbid();

        var p = await context.Products.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == companyId.Value);
        if (p is null) return NotFound();

        p.IsPublished = !p.IsPublished;
        await context.SaveChangesAsync();
        return Ok(new { p.Id, p.IsPublished });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Empresa")]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = GetCompanyIdFromToken();
        if (companyId is null) return Forbid();

        var p = await context.Products.FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == companyId.Value);
        if (p is null) return NotFound();

        context.Products.Remove(p);
        await context.SaveChangesAsync();
        return NoContent();
    }
}
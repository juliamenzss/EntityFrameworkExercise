using System.Drawing.Printing;
using EntityFrameworkExercise.Data;
using EntityFrameworkExercise.Models;
using EntityFrameworkExercise.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkExercise.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(StoreContext context, ILogger<Product> logger) : ControllerBase
{
    // GET: api/Products
    [HttpGet]
    public async Task<IActionResult> GetProducts(int page = 1, int pageSize = 10)
    {
        var productList = await context.Products
            .Include(s => s.Sales)
            .Select(p => new
            {
                p.Id,
                p.Name,
                Price = Math.Round(p.Price, 2),
                Sales = p.Sales
                .OrderBy(s => s.Id)
                .Select(s => new { s.Id })
                .ToList()
            })
        .OrderBy(p => p.Id)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

        return Ok(productList);
    }

    // GET: api/Products/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var productResult = await context.Products
            .Where(p => p.Id == id)
            .Include(s => s.Sales)
            .Select(p => new
            {
                p.Id,
                p.Name,
                Price = Math.Round(p.Price, 2),
                Sales = p.Sales
                .OrderBy(s => s.Id)
                .Select(s => new { s.Id })
            })
            .SingleOrDefaultAsync();

        if (productResult == null)
        {
            logger.LogWarning("The product is null");
            return NotFound();
        }

        return Ok(productResult);
    }

    // PUT: api/Products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(int id, ProductUpdateRequest request)
    {
        var productResult = await context.Products
         .SingleOrDefaultAsync(p => p.Id == id);

        if (productResult == null)
        {
            logger.LogWarning("The product is null");
            return NotFound();
        }

        productResult.Name = request.Name;
        productResult.Price = Math.Round(request.Price, 2);
        try
        {
            await context.SaveChangesAsync();
            return Ok(productResult);
        }
        catch (DbUpdateException dbEx) //database related errors - other option is use 'Exception ex' for anything error in application, but is more generic
        {
            logger.LogError(dbEx, "Database update failed for product ID {Id}", id);
            return BadRequest();
        }
    }

    // POST: api/Products
    [HttpPost]
    public async Task<IActionResult> PostProduct(ProductCreateRequest request)
    {
        var newProduct = new Product
        {
            Name = request.Name,
            Price = Math.Round(request.Price, 2),
        };
        context.Products.Add(newProduct);
        try
        {
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = newProduct.Id }, newProduct);
        }
        catch (DbUpdateException dbEx)
        {
            logger.LogError(dbEx, "Failed to create product");
            return BadRequest();
        }
    }

    // DELETE: api/Products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var productResult = await context.Products
            .SingleOrDefaultAsync(p => p.Id == id);

        if (productResult == null)
        {
            logger.LogWarning("The product is null");
            return NotFound();
        }

        context.Products.Remove(productResult);
        await context.SaveChangesAsync();
        return NoContent();
    }
}
using EntityFrameworkExercise.Data;
using EntityFrameworkExercise.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkExercise.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(StoreContext context) : ControllerBase
{
    // GET: api/Products
    [HttpGet]
    public async Task<IActionResult> GetProducts()
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
            })
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
            .FirstOrDefaultAsync();

        if (productResult == null)
        {
            return NotFound();
        }

        return Ok(productResult);
    }

    // PUT: api/Products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(int id, Product product)
    {
        var productResult = await context.Products
            .FirstOrDefaultAsync(p => p.Id == id);

        if (productResult == null)
        {
            return NotFound();
        }

        productResult.Name = product.Name;
        productResult.Price = product.Price;

        await context.SaveChangesAsync();

        return Ok(productResult);
    }

    // POST: api/Products
    [HttpPost]
    public async Task<IActionResult> PostProduct(Product product)
    {
        var newProduct = new Product
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Sales = product.Sales
        };

        context.Products.Add(newProduct);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(PostProduct), new { id = newProduct.Id }, newProduct);

    }

    // DELETE: api/Products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var productResult = await context.Products
            .FirstOrDefaultAsync(p => p.Id == id);

        if(productResult == null)
        {
            return NotFound();
        }

        context.Products.Remove(productResult);
        await context.SaveChangesAsync();
        return NoContent();
    }
}
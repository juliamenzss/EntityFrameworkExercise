using System.Drawing.Printing;
using EntityFrameworkExercise.Data;
using EntityFrameworkExercise.Models;
using EntityFrameworkExercise.Requests;
using EntityFrameworkExercise.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.Swagger.Annotations;

namespace EntityFrameworkExercise.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(StoreContext context, ILogger<Product> logger) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("Return a list of Product")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductResponse>))]
    public async Task<IActionResult> GetProducts(int page = 1, int pageSize = 10)
    {
        var productList = await context.Products
            .Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Price = Math.Round(p.Price, 2),
                Sales = p.Sales.Count
            })
        .OrderBy(p => p.Id)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

        return Ok(productList);
    }

    [HttpGet("{id}")]
    [SwaggerOperation("Get a product by id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct(int id)
    {
        var productResult = await context.Products
            .Where(p => p.Id == id)
            .Include(s => s.Sales)
            .Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Price = Math.Round(p.Price, 2),
                Sales = p.Sales.Count
            })
            .SingleOrDefaultAsync();

        if (productResult == null)
        {
            logger.LogWarning("The product is null");
            return NotFound();
        }
        return Ok(productResult);
    }

    [HttpPut("{id}")]
    [SwaggerOperation("Update product by id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutProduct(int id, ProductUpdateRequest request)
    {
        try
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

            await context.SaveChangesAsync();
            return Ok(productResult);
        }
        catch (DbUpdateException dbEx)
        {
            logger.LogError(dbEx, "Database update failed");
            return BadRequest();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in update");
            return BadRequest();
        }
    }

    [HttpPost]
    [SwaggerOperation("Create a new product")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostProduct(ProductCreateRequest request)
    {
        try
        {
            var newProduct = new Product
            {
                Name = request.Name,
                Price = Math.Round(request.Price, 2),
            };
            context.Products.Add(newProduct);

            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), "Product", new { id = newProduct.Id }, newProduct);
        }
        catch (DbUpdateException dbEx)
        {
            logger.LogError(dbEx, "Failed to create product");
            return BadRequest();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create product");
            return BadRequest();
        }
    }

    [HttpDelete("{id}")]
    [SwaggerOperation("Delete product by id")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        await context.Products.Where(p => p.Id == id).ExecuteDeleteAsync();
        return NoContent();
    }
}
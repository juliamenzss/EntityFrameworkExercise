using EntityFrameworkExercise.Data;
using EntityFrameworkExercise.Models;
using EntityFrameworkExercise.Requests;
using EntityFrameworkExercise.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.Swagger.Annotations;

namespace EntityFrameworkExercise.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SellersController(StoreContext context, ILogger<Seller> logger) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("Return a list of Seller")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SellerResponse>))]
    public async Task<IActionResult> GetSellers(int page = 1, int pageSize = 10)
    {
        var sellerList = await context.Sellers
            .Include(s => s.Sales)
            .Select(s => new SellerResponse
            {
                Id = s.Id,
                Name = s.Name,
                Sales = s.Sales.Count
            })
            .OrderBy(s => s.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(sellerList);
    }


    [HttpGet("{id}")]
    [SwaggerOperation("Get seller by id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSeller(int id)
    {
        var sellerResult = await context.Sellers
            .Where(s => s.Id == id)
            .Select(s => new SellerResponse
            {
                Id = s.Id,
                Name = s.Name,
                Sales = s.Sales.Count
            })
            .SingleOrDefaultAsync();

        if (sellerResult == null)
        {
            logger.LogWarning("The seller is null");
            return NotFound();
        }
        return Ok(sellerResult);
    }

    [HttpPut("{id}")]
    [SwaggerOperation("Update seller by id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutSeller(int id, SellerUpdateRequest request)
    {
        try
        {
            var sellerResult = await context.Sellers
            .SingleOrDefaultAsync(s => s.Id == id);

            if (sellerResult == null)
            {
                logger.LogError("The seller is null");
                return NotFound();
            }

            sellerResult.Name = request.Name;
            await context.SaveChangesAsync();
            return Ok(sellerResult);
        }
        catch (DbUpdateException dbEx)
        {
            logger.LogError(dbEx, "Database update failed");
            return BadRequest();
        }  
        catch (Exception ex)
        {
            logger.LogError(ex, "Error to update");
            return BadRequest();
        }

    }
    [HttpPost]
    [SwaggerOperation("Create a new seller")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostSeller(SellerCreateRequest request)
    {
        try
        {
            var newSeller = new Seller
            {
                Name = request.Name,
            };
            context.Sellers.Add(newSeller);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSeller), "Seller", new { id = newSeller.Id }, newSeller);
        }
        catch (DbUpdateException dbEx)
        {
            logger.LogError(dbEx, "Failed DB");
            return BadRequest();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create seller");
            return BadRequest();
        }
    }

    [HttpDelete("{id}")]
    [SwaggerOperation("Delete seller by id")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSeller(int id)
    {
        await context.Sellers.Where(s => s.Id == id).ExecuteDeleteAsync();
        return NoContent();
    }
}
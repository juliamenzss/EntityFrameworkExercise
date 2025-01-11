using EntityFrameworkExercise.Data;
using EntityFrameworkExercise.Models;
using EntityFrameworkExercise.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkExercise.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SellersController(StoreContext context, ILogger<Seller> logger) : ControllerBase
{
    // GET: api/Sellers
    [HttpGet]
    public async Task<IActionResult> GetSellers(int page = 1, int pageSize = 10)
    {
        var sellerList = await context.Sellers
            .Include(s => s.Sales)
            .Select(s => new
            {
                s.Id,
                s.Name,
                Sales = s.Sales
                .OrderBy(s => s.Id)
                .Select(x => new { x.Id })
            })
            .OrderBy(s => s.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(sellerList);
    }


    // GET: api/Sellers/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSeller(int id)
    {
        var sellerResult = await context.Sellers
            .Where(s => s.Id == id)
            .Include(s => s.Sales)
            .Select(s => new
            {
                s.Id,
                s.Name,
                Sales = s.Sales
                .Select(s => new { s.Id })
                .OrderBy(s => s.Id)
                .ToList()
            })
            .SingleOrDefaultAsync();

        if (sellerResult == null)
        {
            logger.LogWarning("The seller is null");
            return NotFound();
        }
        return Ok(sellerResult);
    }

    // PUT: api/Sellers/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSeller(int id, SellerUpdateRequest request)
    {
        var sellerResult = await context.Sellers
            .SingleOrDefaultAsync(s => s.Id == id);

        if (sellerResult == null)
        {
            logger.LogError("The seller is null");
            return BadRequest();
        }

        sellerResult.Name = request.Name;

        try
        {
            await context.SaveChangesAsync();
            return Ok(sellerResult);
        }
        catch (DbUpdateException dbEx)
        {
            logger.LogError(dbEx, "Database update failed for seller ID {Id}", id);
            return BadRequest();
        }

    }
    // POST: api/Sellers
    [HttpPost]
    public async Task<IActionResult> PostSeller(SellerCreateRequest request)

    {
        var newSeller = new Seller
        {
            Name = request.Name,
        };
        context.Sellers.Add(newSeller);
        try
        {
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSeller), new { id = newSeller.Id }, newSeller);
        }
        catch (DbUpdateException dbEx)
        {
            logger.LogError(dbEx, "Failed to create seller");
            return BadRequest();
        }
    }

    // DELETE: api/Sellers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSeller(int id)
    {
        var sellerResult = await context.Sellers
            .SingleOrDefaultAsync(s => s.Id == id);
        if (sellerResult == null)
        {
            logger.LogWarning("The seller is null");
            return NotFound();
        }
        context.Sellers.Remove(sellerResult);
        await context.SaveChangesAsync();
        return NoContent();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkExercise.Data;
using EntityFrameworkExercise.Models;

namespace EntityFrameworkExercise.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SalesController(StoreContext context, ILogger<Sale> logger) : ControllerBase
{
    // GET: api/Sales
    [HttpGet]
    public async Task<IActionResult> GetSales(int page = 1, int pageSize = 10)
    {

        var totalCount = await context.Sales.CountAsync();
        var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

        var listResult = await context.Sales
            .Select(s => new
            {
                s.Id,
                s.Date,
                Seller = new
                {
                    s.Seller.Id,
                    s.Seller.Name
                },
                Customer = new
                {
                    s.Customer.Id,
                    s.Customer.Name
                },
                Products = s.Products.Select(p => new
                {
                    p.Id,
                    p.Name,
                    Price = Math.Round(p.Price, 2)
                })
                .OrderBy(p => p.Id)
                .ToList()
            })
            .OrderBy(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return Ok(listResult);
    }

    // GET: api/Sales/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSale(int id)
    {
        var saleResult = await context.Sales
        .Where(s => s.Id == id)
        .Select(s => new
        {
            s.Id,
            s.Date,
            Seller = new
            {
                s.Seller.Id,
                s.Seller.Name
            },
            Customer = new
            {
                s.Customer.Id,
                s.Customer.Name
            },
            Products = s.Products.Select(p => new
            {
                p.Id,
                p.Name,
                Price = Math.Round(p.Price, 2)
            })
                .OrderBy(p => p.Id)
                .ToList()
        })
        .SingleOrDefaultAsync();

        if (saleResult == null)
        {
            logger.LogWarning("The sale is null");
            return NotFound();
        }

        return Ok(saleResult);
    }

    // PUT: api/Sales/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSale(int id, Sale sale)
    {

        if (id != sale.Id)
        {
            logger.LogWarning("Sale not found");
            return NotFound();
        }

        var saleResult = await context.Sales
        .Include(s => s.Products)
        .SingleOrDefaultAsync(s => s.Id == id);

        if (saleResult == null)
        {
            logger.LogWarning("Sale not found: {Id}", id);
            return NotFound();
        }

        saleResult.Date = sale.Date;
        saleResult.SellerId = sale.SellerId;
        saleResult.CustomerId = sale.CustomerId;

        saleResult.Products.Clear();
        foreach (var product in sale.Products)
        {
            saleResult.Products.Add(product);
        }

        try
        {
            await context.SaveChangesAsync();
            return Ok(saleResult);
        }
        catch (DbUpdateException dbEx)
        {
            logger.LogError(dbEx, "Database update failed for sale {Id}", id);
            return BadRequest();
        }
    }

    // POST: api/Sales
    [HttpPost]
    public async Task<IActionResult> PostSale(Sale sale)
    {
        var newSale = new Sale
        {
            SellerId = sale.SellerId,
            CustomerId = sale.CustomerId,
            Products = sale.Products
        };
        context.Sales.Add(newSale);
        try
        {
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(PostSale), new { id = newSale.Id }, newSale);
        }
        catch (DbUpdateException dbEx)
        {
            logger.LogError(dbEx, "Error creating sale");
            return BadRequest();
        }
    }

    // DELETE: api/Sales/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSale(int id)
    {
        var sale = await context.Sales
                .SingleOrDefaultAsync(s => s.Id == id);

        if (sale == null)
        {
            logger.LogWarning("The sale is null");
            return NotFound();
        }

        context.Sales.Remove(sale);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

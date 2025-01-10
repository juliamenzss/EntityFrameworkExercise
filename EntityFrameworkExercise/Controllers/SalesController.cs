using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkExercise.Data;
using EntityFrameworkExercise.Models;
using EntityFrameworkExercise.Requests;

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
    public async Task<IActionResult> PutSale(int id, SaleUpdateRequest request)
    {

        var sale = await context.Sales.SingleOrDefaultAsync(sale => sale.Id == id);
        if (sale == null)
        {
            logger.LogWarning("The sale is null");
            return NotFound();
        }

        sale.Date = request.Date;
        sale.SellerId = request.SellerId;
        sale.CustomerId = request.CustomerId;

        try
        {
            await context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error to update sale");
            return BadRequest("Error to uptade sale");
        }


    }
    // POST: api/Sales
    [HttpPost]
    public async Task<IActionResult> PostSale(SaleCreateRequest request)
    {
        if (request == null)
        {
            return BadRequest("Request is null");
        }
        try
        {
            var seller = await context.Sellers.FirstOrDefaultAsync(s => s.Id == request.SellerId);
            if (seller == null)
            {
                logger.LogWarning("Seller is null");
                return NotFound();
            }

            var customer = await context.Customers.FirstOrDefaultAsync(c => c.Id == request.CustomerId);
            if (customer == null)
            {
                logger.LogWarning("Customer is null");
                return NotFound();
            }

            if (request.Products == null || request.Products.Count == 0)
            {
                logger.LogWarning("Product List is null or empty");
                return NotFound();
            }

            var newSale = new Sale
            {
                Date = request.Date,
                SellerId = request.SellerId,
                CustomerId = request.CustomerId,
            };

            context.Sales.Add(newSale);
            await context.SaveChangesAsync();

            foreach (var product in request.Products)
            {
                var productSale = new ProductSale
                {
                    ProductId = product.Id,
                    SaleId = newSale.Id
                };
            }


            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error to create sale");
            return BadRequest("Error to create sale");
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

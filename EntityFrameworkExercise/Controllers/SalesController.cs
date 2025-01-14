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
using Swashbuckle.Swagger.Annotations;
using EntityFrameworkExercise.Response;
using System.Data.Common;

namespace EntityFrameworkExercise.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SalesController(StoreContext context, ILogger<Sale> logger) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("Return a list of Sale")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SaleResponse>))]
    public async Task<IActionResult> GetSales(int page = 1, int pageSize = 10)
    {

        var totalCount = await context.Sales.CountAsync();
        var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

        var listResult = await context.Sales
            .Select(s => new SaleResponse
            {
                Id = s.Id,
                Date = s.Date,
                CustomerId = s.CustomerId,
                SellerId = s.SellerId,
                Product = s.Products.Count

                //Seller = new SellerResponse
                //{
                //    Id = s.Seller.Id,
                //    Name = s.Seller.Name
                //},
                //Customer = new CustomerResponse
                //{
                //    Id = s.Customer.Id,
                //    Name = s.Customer.Name
                //},
                //Products = s.Products.Select(p => new
                //{
                //    p.Id,
                //    p.Name,
                //    Price = Math.Round(p.Price, 2)
                //})
                //.OrderBy(p => p.Id)
                //.ToList()
            })
            .OrderBy(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return Ok(listResult);
    }

    [HttpGet("{id}")]
    [SwaggerOperation("Get sale by id")]
    [ProducesResponseType<Sale>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSale(int id)
    {
        var saleResult = await context.Sales
        .Where(s => s.Id == id)
        .Select(s => new SaleResponse
        {
            Id = s.Id,
            Date = s.Date,
            Seller = new SellerResponse
            {
                Id = s.Seller.Id,
                Name = s.Seller.Name
            },
            Customer = new CustomerResponse
            {
                Id = s.Customer.Id,
                Name = s.Customer.Name
            },
            Product = s.Products.Count
        })
        .SingleOrDefaultAsync();

        if (saleResult == null)
        {
            logger.LogWarning("The sale is null");
            return NotFound();
        }
        return Ok(saleResult);
    }

    [HttpPut("{id}")]
    [SwaggerOperation("Update sale by id")]
    [ProducesResponseType<Sale>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutSale(int id, SaleUpdateRequest request)
    {
        try
        {
            var sale = await context.Sales.SingleOrDefaultAsync(sale => sale.Id == id);
            if (sale == null)
            {
                logger.LogWarning("The sale is null");
                return NotFound();
            }

            sale.SellerId = request.SellerId;
            sale.CustomerId = request.CustomerId;

            await context.SaveChangesAsync();
            return Ok(sale);
        }
        catch (DbUpdateException dbEx)
        {
            logger.LogError(dbEx, "Failed DB");
            return BadRequest();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error to update sale");
            return BadRequest("Error to uptade sale");
        }
    }
    [HttpPost]
    [SwaggerOperation("Create a new sale")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PostSale(SaleCreateRequest request)
    {
        try
        {
            var customer = await context.Customers.FirstOrDefaultAsync(c => c.Id == request.CustomerId);
            if (customer == null)
            {
                logger.LogWarning("Customer is null");
                return NotFound();
            }

            var seller = await context.Sellers.FirstOrDefaultAsync(s => s.Id == request.SellerId);
            if (seller == null)
            {
                logger.LogWarning("Seller is null");
                return NotFound();
            }

            var products = await context.Products
            .Where(p => request.Products
            .Contains(p.Id))
            .ToListAsync();

            if (products.Count == 0)
            {
                logger.LogWarning("Seller is null or empty");
                return NotFound();
            }

            var newSale = new Sale
            {
                SellerId = request.SellerId,
                CustomerId = request.CustomerId,
                Products = products
            };

            context.Sales.Add(newSale);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSale), "Sales", new { id = newSale.Id }, null);
        }
        catch (DbException dbEx)
        {
            logger.LogError(dbEx, "Error DB");
            return BadRequest("Error DB");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error to create sale");
            return BadRequest("Error to create sale");
        }
    }

    [HttpDelete("{id}")]
    [SwaggerOperation("Delete sale by id")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSale(int id)
    {
        await context.Sales.Where(s => s.Id == id).ExecuteDeleteAsync();
        return NoContent();
    }
}

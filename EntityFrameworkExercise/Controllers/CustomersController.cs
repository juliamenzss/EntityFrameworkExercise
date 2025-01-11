using EntityFrameworkExercise.Data;
using EntityFrameworkExercise.Models;
using EntityFrameworkExercise.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EntityFrameworkExercise.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController(StoreContext context, ILogger<Customer> logger) : ControllerBase
{
    // GET: api/Customers
    [HttpGet]
    public async Task<IActionResult> GetCustomers(int page = 1, int pageSize = 10)
    {
        logger.LogInformation("I am an Information");
        logger.LogWarning("I am a Warning");
        logger.LogInformation("I am an Error");

        var listResult = await context.Customers
            .Include(x => x.Sales)
            .Select(c => new
            {
                c.Id,
                c.Name,
                Sales = c.Sales.Select(s => new
                { s.Id })
            })
            .OrderBy(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return Ok(listResult);
    }

    // GET: api/Customers/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomer(int id)
    {

        var customerResult = await context.Customers
            .Where(c => c.Id == id) //search the customer for Id
            .Include(c => c.Sales) //include Sales in the search
            .Select(c => new //for each 'c' create a new object
            {
                c.Id,
                c.Name,
                Sales = c.Sales
                .Select(s => new { s.Id }) //include de Id of Sales
            })
            .SingleOrDefaultAsync();

        if (customerResult == null)
        {
            logger.LogWarning("The customer is null");
            return NotFound();
        }
        return Ok(customerResult);
    }


    // PUT: api/Customers/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCustomer(int id, CustomerCreateRequest request)
    {
        try
        {
            var customer = await context.Customers
                       .SingleOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                logger.LogWarning("The customer is null");
                return NotFound();
            }

            customer.Name = request.Name;

            await context.SaveChangesAsync();

            return Ok(customer);
        }
        catch (DbUpdateException dbEx)
        {
            logger.LogError(dbEx, "Database update failed for customer ID {Id}", id);
            return BadRequest();
        }
    }

    // POST: api/Customers
    [HttpPost]
    public async Task<IActionResult> PostCustomer(CustomerCreateRequest request)
    {
        var newCustomer = new Customer
        {
            Name = request.Name,
        };

        context.Customers.Add(newCustomer);

        try
        {
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCustomer), new { id = newCustomer.Id }, newCustomer);
        }
        catch (DbUpdateException dbEx)
        {
            logger.LogError(dbEx, "Failed to create customer");
            return BadRequest();
        }
    }

    // DELETE: api/Customers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var customerResult = await context.Customers
            .SingleOrDefaultAsync(c => c.Id == id);

        if (customerResult == null)
        {
            logger.LogWarning("The customer is null");
            return NotFound();
        }

        context.Customers.Remove(customerResult);
        await context.SaveChangesAsync();
        return NoContent();
    }
}
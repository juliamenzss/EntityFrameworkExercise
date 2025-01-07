using EntityFrameworkExercise.Data;
using EntityFrameworkExercise.Models;
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
    public async Task<IActionResult> GetCustomers()
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
                Sales = c.Sales.Select(s => new {s.Id})
            })
            .ToListAsync();
        return Ok(listResult);
    }

    // GET: api/Customers/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomer(int id)
    {
     
        var listResult = await context.Customers
            .Where(c => c.Id == id) //search the customer for Id
            .Include(c => c.Sales) //include Sales in the search
            .Select(c => new //for each 'c' create a new object
            {
                c.Id,
                c.Name,
                Sales = c.Sales
                .Select(s => new { s.Id }) //include de Id of Sales
            })
            .FirstOrDefaultAsync();

        if(listResult == null)
        {
            return NotFound();
        }
        return Ok(listResult);
    }


    // PUT: api/Customers/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCustomer(int id, Customer customer)
    {
        var customerResult = await context.Customers
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customerResult == null)
        {
            return NotFound();
        }

        customerResult.Name = customer.Name;

        await context.SaveChangesAsync();

        return Ok(customerResult);
    }

    // POST: api/Customers
    [HttpPost]
    public async Task<IActionResult> PostCustomer(Customer customer)
    {
        var newCustomer = new Customer
        {
            Id = customer.Id,
            Name = customer.Name,
            Sales = customer.Sales
        };

        context.Customers.Add(newCustomer);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(PostCustomer), new { id = newCustomer.Id }, newCustomer);
    }

    // DELETE: api/Customers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var customerResult = await context.Customers
            .FirstOrDefaultAsync(c => c.Id == id);

        if(customerResult == null)
        {
            return NotFound();
        }

        context.Customers.Remove(customerResult);
        await context.SaveChangesAsync();
        return NoContent();
    }
}
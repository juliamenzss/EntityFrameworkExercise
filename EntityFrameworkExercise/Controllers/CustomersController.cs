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
public class CustomersController(StoreContext context, ILogger<Customer> logger) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("Return a list of Customer")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CustomerResponse>))]
    public async Task<IActionResult> GetCustomers(int page = 1, int pageSize = 10)
    {
        var listResult = await context.Customers
            .Select(c => new CustomerResponse
            {
                Id = c.Id,
                Name = c.Name,
                Sales = c.Sales.Count
            })
            .OrderBy(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return Ok(listResult);
    }

    [HttpGet("{id}")]
    [SwaggerOperation("Get a customer by id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomer(int id)
    {

        var customerResult = await context.Customers
            .Where(c => c.Id == id)
            .Select(c => new CustomerResponse
            {
                Id = c.Id,
                Name = c.Name,
                Sales = c.Sales.Count
            })
            .SingleOrDefaultAsync();

        if (customerResult == null)
        {
            logger.LogWarning("The customer is null");
            return NotFound();
        }
        return Ok(customerResult);
    }


    [HttpPut("{id}")]
    [SwaggerOperation("Update customer by id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutCustomer(int id, CustomerUpdateRequest request)
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
            logger.LogError(dbEx, "Database update failed");
            return BadRequest();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error update failed");
            return BadRequest();
        }
    }

    [HttpPost]
    [SwaggerOperation("Create a new customer")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostCustomer(CustomerCreateRequest request)
    {
        try
        {
            var newCustomer = new Customer
            {
                Name = request.Name,
            };

            context.Customers.Add(newCustomer);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCustomer), new { id = newCustomer.Id }, newCustomer);
        }
        catch (DbUpdateException dbEx)
        {
            logger.LogError(dbEx, "Error in DB");
            return BadRequest();

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create customer");
            return BadRequest();
        }
    }

    [HttpDelete("{id}")]
    [SwaggerOperation("Delete customer by id")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        await context.Customers.Where(c => c.Id == id).ExecuteDeleteAsync();
        return NoContent();
    }
}
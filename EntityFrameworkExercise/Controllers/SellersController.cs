using EntityFrameworkExercise.Data;
using EntityFrameworkExercise.Models;
using Microsoft.AspNetCore.Mvc;

namespace EntityFrameworkExercise.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SellersController(StoreContext context) : ControllerBase
{
    // GET: api/Sellers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Seller>>> GetSellers()
    {
        return default;
    }

    // GET: api/Sellers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Seller>> GetSeller(int id)
    {
        return default;
    }

    // PUT: api/Sellers/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSeller(int id, Seller seller)
    {
        return default;
    }

    // POST: api/Sellers
    [HttpPost]
    public async Task<ActionResult<Seller>> PostSeller(Seller seller)
    {
        return default;
    }

    // DELETE: api/Sellers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSeller(int id)
    {
        return default;
    }
}
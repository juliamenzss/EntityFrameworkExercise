using EntityFrameworkExercise.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFrameworkExercise.Requests
{
    public class SaleCreateRequest
    {
        public DateTimeOffset Date { get; set; }
        public int SellerId { get; set; }
        public int CustomerId { get; set; } = default!;
        public List<Product> Products { get; set; } = default!;
    }
}

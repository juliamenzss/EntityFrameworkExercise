using EntityFrameworkExercise.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFrameworkExercise.Requests
{
    public class SaleCreateRequest
    {
        public int SellerId { get; set; }
        public int CustomerId { get; set; } = default!;
        public List<int> Products { get; set; } = new();
    }
}

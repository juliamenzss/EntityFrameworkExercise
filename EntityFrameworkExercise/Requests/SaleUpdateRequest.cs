using EntityFrameworkExercise.Models;

namespace EntityFrameworkExercise.Requests
{
    public class SaleUpdateRequest
    {
        public DateTimeOffset Date { get; set; }
        public int SellerId { get; set; }
        public int CustomerId { get; set; } = default!;
        public List<Product> Products { get; set; } = default!; // isso fica ou sai?
    }
}

using EntityFrameworkExercise.Models;

namespace EntityFrameworkExercise.Requests
{
    public class SaleUpdateRequest
    {
        public int SellerId { get; set; }
        public int CustomerId { get; set; } = default!;
    }
}

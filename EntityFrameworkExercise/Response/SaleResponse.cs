using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkExercise.Response
{
    public class SaleResponse
    {
        public int Id { get; set; }

        [Required]
        public DateTimeOffset Date { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int SellerId { get; set; }
        public int Product { get; set; } = default!;
        public SellerResponse Seller { get; set; } = default!;
        public CustomerResponse Customer { get; set; } = default!;
    }
}

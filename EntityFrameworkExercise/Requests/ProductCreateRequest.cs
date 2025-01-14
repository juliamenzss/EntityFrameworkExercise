using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkExercise.Requests
{
    public class ProductCreateRequest
    {
        [MinLength(5), MaxLength(20)]
        public string Name { get; set; } = string.Empty!;
        [Required]
        public decimal Price { get; set; } = default!;
    }
}

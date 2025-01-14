using EntityFrameworkExercise.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFrameworkExercise.Requests
{
    public class ProductUpdateRequest
    {
        [MinLength(5), MaxLength(20)]
        public string Name { get; set; } = string.Empty!;
        
        [Required]
        public decimal Price { get; set; } = default!;
    }
}

using EntityFrameworkExercise.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFrameworkExercise.Requests
{
    public class ProductUpdateRequest
    {
        public string Name { get; set; } = string.Empty!;
        public decimal Price { get; set; } = default!;
    }
}

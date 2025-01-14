using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkExercise.Requests
{
    public class SellerUpdateRequest
    {
        [MinLength(5), MaxLength(45)]
        public string Name { get; set; } = string.Empty!;
    }
}

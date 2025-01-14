using System.ComponentModel.DataAnnotations;
using EntityFrameworkExercise.Models;

namespace EntityFrameworkExercise.Requests
{
    public class SaleUpdateRequest
    {
        [Required]
        public int SellerId { get; set; }

        [Required]
        public int CustomerId { get; set; }
    }
}

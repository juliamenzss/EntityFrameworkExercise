using EntityFrameworkExercise.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFrameworkExercise.Requests
{
    public class SaleCreateRequest
    {
        [Required]
        public int SellerId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public IEnumerable<int> Products { get; set; } = [];
    }
}

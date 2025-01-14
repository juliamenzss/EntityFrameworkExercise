using System.ComponentModel.DataAnnotations;
using EntityFrameworkExercise.Models;

namespace EntityFrameworkExercise.Requests
{
    public class CustomerCreateRequest
    {
        [MinLength(5), MaxLength(45)]
        public string Name { get; set; } = string.Empty!;
    }
}

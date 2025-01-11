using EntityFrameworkExercise.Models;

namespace EntityFrameworkExercise.Requests
{
    public class CustomerUpdateRequest
    {
        public string Name { get; set; } = string.Empty!;
        //public List<Sale> Sales { get; set; } = default!;
    }
}

namespace EntityFrameworkExercise.Response
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required decimal Price { get; set; }
        public int Sales { get; set; }
    }
}

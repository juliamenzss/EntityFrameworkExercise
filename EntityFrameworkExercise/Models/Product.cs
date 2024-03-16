using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkExercise.Models;

[Table("product")]
public class Product
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty!;

    [Column("price")]
    public decimal Price { get; set; } = default!;

    public List<Sale> Sales { get; set; } = default!;
}
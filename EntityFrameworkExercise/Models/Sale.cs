using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkExercise.Models;

[Table("sale")]
public class Sale
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("date")]
    public DateTimeOffset Date { get; set; }

    [Column("seller_id")]
    public int SellerId { get; set; }

    public Seller Seller { get; set; } = default!;

    [Column("customer_id")]
    public int CustomerId { get; set; } = default!;

    public Customer Customer { get; set; } = default!;
    public List<Product> Products { get; set; } = default!;
}
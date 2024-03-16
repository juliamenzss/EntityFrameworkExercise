using Bogus;
using EntityFrameworkExercise.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFrameworkExercise.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMockProductSale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            new Faker<ProductSale>()
               .RuleFor(x => x.ProductId, x => x.Random.Int(1, 100))
               .RuleFor(x => x.SaleId, x => x.Random.Int(1, 100))
               .Generate(10000)
               .ForEach(ps =>
               {
                   migrationBuilder.InsertData(
                      schema: "store",
                      table: "product_sale",
                      columns: ["product_id", "sale_id"],
                      values: new object[,] { { ps.ProductId, ps.SaleId, } });
               });
        }
    }
}
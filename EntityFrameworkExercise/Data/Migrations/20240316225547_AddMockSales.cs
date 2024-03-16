using Bogus;
using EntityFrameworkExercise.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFrameworkExercise.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMockSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            new Faker<Sale>()
               .RuleFor(x => x.SellerId, x => x.Random.Int(1, 5))
               .RuleFor(x => x.CustomerId, x => x.Random.Int(1, 5))
               .RuleFor(x => x.Date, x => x.Date.Between(DateTime.Now.AddDays(-10), DateTime.Now))
               .Generate(100)
               .ForEach(s =>
               {
                   migrationBuilder.InsertData(
                      schema: "store",
                      table: "sale",
                      columns: ["date", "customer_id", "seller_id"],
                      values: new object[,] { { s.Date, s.CustomerId, s.SellerId } });
               });
        }
    }
}
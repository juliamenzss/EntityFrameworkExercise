using Bogus;
using EntityFrameworkExercise.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFrameworkExercise.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMockEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            new Faker<Product>("pt_BR")
               .RuleFor(x => x.Name, x => x.Commerce.Product())
               .RuleFor(x => x.Price, x => x.Random.Decimal(0.0m, 100.1m))
               .Generate(100)
               .ForEach(p =>
               {
                   migrationBuilder.InsertData(
                       schema: "store",
                       table: "product",
                       columns: ["name", "price"],
                       values: new object[,] { { p.Name, p.Price } });
               });

            new Faker<Seller>("pt_BR")
                .RuleFor(x => x.Name, x => x.Person.FullName)
                .Generate(5)
                .ForEach(s =>
                {
                    migrationBuilder.InsertData(
                        schema: "store",
                        table: "seller",
                        columns: ["name"],
                        values: new object[,] { { s.Name } });
                });

            new Faker<Customer>("pt_BR")
                .RuleFor(x => x.Name, x => x.Person.FirstName)
                .Generate(5)
                .ForEach(c =>
                {
                    migrationBuilder.InsertData(
                        schema: "store",
                        table: "customer",
                        columns: ["name"],
                        values: new object[,] { { c.Name } });
                });
        }
    }
}
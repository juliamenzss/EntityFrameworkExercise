using EntityFrameworkExercise.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.
var services = builder.Services;

var connectionString = configuration.GetConnectionString("StoreContext")
    ?? throw new InvalidOperationException("Connection string 'StoreContext' not found.");

services.AddDbContext<StoreContext>(options =>
{
    options.UseSqlite(connectionString, p =>
    {
        p.MigrationsHistoryTable("__EFMigrationsHistory", "store");
    });
});

services.AddControllers();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sua API",
        Version = "v1",
        Description = "Descrição da sua API"
    });

    c.EnableAnnotations();

    c.CustomSchemaIds(type => type.FullName);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
    context.Database.Migrate();
}
app.Run();
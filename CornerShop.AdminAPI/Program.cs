using CornerShop.Core;
using CornerShop.Core.Data;
using CornerShop.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace CornerShop.AdminAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ShopRepository repo=new ShopRepository(DBConfig.ConnectionString);

            // Add services to the container.

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddAuthorization();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
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

            //POST
            app.MapPost("/products", async ([FromBody] Product product) =>
            {
                await repo.AddProductAsync(product);
                return Results.Created($"/products/{product.Id}", product);
            });
            //GET ALL 
            app.MapGet("/products", async () =>
            {
                var products = await repo.GetAllProductsAsync();
                return Results.Ok(products);
            });
            //GET PRODUCT BY ID
            app.MapGet("/products/{id}", async (string id) =>
            {
                var product = await repo.GetProductByIdAsync(id);
                if (product == null) return Results.NotFound("Product not found");
                return Results.Ok(product);
            });
            //GET PRODUCT BY CATEGORY
            app.MapGet("/products/category/{category}", async (ProductCategory category) =>
            {
                var products = await repo.GetProductsByCategoryAsync(category);
                return Results.Ok(products);
            });
            //DELETE 
            app.MapDelete("/products/{id}", async (string id) =>
            {
                await repo.DeleteProductAsync(id);
                return Results.Ok($"Request to delete ID {id} sent.");
            });
            //UPDATE
            app.MapPut("/products/", async (string id, Product product) =>
            {
                if (id != product.Id)
                {
                    return Results.BadRequest("ID mismatch: URL ID does not match Body ID.");
                }
                var existing = await repo.GetProductByIdAsync(id);
                if (existing == null)
                {
                    return Results.NotFound("Product not found");
                }
                await repo.UpdateProductAsync(product);
                return Results.Ok(product);
            });

            app.Run();
        }
    }
}

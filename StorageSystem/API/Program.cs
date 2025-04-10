using Microsoft.EntityFrameworkCore;
using StorageSystem;
using StorageSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer(); // Adds support for API endpoints
builder.Services.AddSwaggerGen(); // Generates Swagger documentation

builder.Services.AddControllers(); // Registers controllers

var app = builder.Build();

// Generate fake data -- This is not permanent.
FakerService.Generate();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Enables Swagger in development mode
    app.UseSwaggerUI(); // Enables Swagger UI in development mode
}

app.UseHttpsRedirection(); // Redirects HTTP requests to HTTPS

app.MapControllers(); // Maps incoming requests to controllers

app.Run();

FakerService.GenerateAndPopulate(100, 50); // Generates and populates the database with fake data
public partial class Program { }

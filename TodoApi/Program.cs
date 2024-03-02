using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Linq;
using TodoApi;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql("server=localhost;database=tasks;user=maayan;password=maayan",
        new MySqlServerVersion(new Version(8, 0, 36))));


// Add CORS policy
builder.Services.AddCors(options =>
{
     options.AddPolicy("AllowAnyOrigin",
         builder =>
         {
              builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
         });
});

// Register Swagger services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Define your routes

//     Console.ForegroundColor = ConsoleColor.Green;
//     Console.WriteLine("********************************************");
//     Console.WriteLine("*               My Server Work             *");
//     Console.WriteLine("********************************************");
//     Console.ResetColor();  
app.MapGet("/", (HttpContext context) =>
{
    context.Response.ContentType = "text/html";

    return @"
        <!DOCTYPE html>
        <html lang=""en"">
        <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>My Server Work</title>
            <style>
                body {
                    font-family: Arial, sans-serif;
                    background-color: #f0f0f0;
                    padding: 20px;
                }
                .container {
                    background-color: #fff;
                    border: 2px solid #333;
                    border-radius: 5px;
                    padding: 20px;
                    text-align: center;
                }
            </style>
        </head>
        <body>
            <div class=""container"">
                <h1>My Server Work</h1>
                <h2>let`s run todo list I will care about it :)</h2>
            </div>
        </body>
        </html>
    ";
});

app.MapGet("/tasks", async ([FromServices] ToDoDbContext _context) =>
{
     var items = await _context.items.ToListAsync();
     return Results.Ok(items);
});

app.MapPost("/tasks", async (Item item, [FromServices] ToDoDbContext _context) =>
{
    var itemToAdd = new Item { Name = item.Name ,IsComplete=false};
    var newItem = await _context.items.AddAsync(itemToAdd);
    await _context.SaveChangesAsync();
    return Results.Ok(newItem.Entity);
});


app.MapPut("/tasks/{id}", async (int id,[FromServices] ToDoDbContext _context) =>
{
    
     var itemToUpdate = await _context.items.FindAsync(id);
    if (itemToUpdate == null)
    {
        return Results.NotFound();
    }

    itemToUpdate.IsComplete = ! itemToUpdate.IsComplete;

    await _context.SaveChangesAsync();
    return Results.Ok(itemToUpdate);
});

app.MapDelete("/tasks/{id}", async (int id, [FromServices] ToDoDbContext _context) =>
{
     var item = await _context.items.FindAsync(id);
     if (item == null)
     {
          return Results.NotFound();
     }

     _context.items.Remove(item);
     await _context.SaveChangesAsync();
     return Results.Ok(item);
});


//  Swagger
//http://localhost:5163/swagger/index.html
if (app.Environment.IsDevelopment())
{
     app.UseSwagger();
     app.UseSwaggerUI();
}
// Apply CORS middleware
app.UseCors("AllowAnyOrigin");

app.Run();

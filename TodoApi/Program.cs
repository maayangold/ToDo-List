using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TodoApi;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

var key = GenerateStrongKey();

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql("server=localhost;database=tasks;user=maayan;password=maayan",
        new MySqlServerVersion(new Version(8, 0, 36))));

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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var app = builder.Build();

app.MapGet("/", (HttpContext context) =>
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("********************************************");
    Console.WriteLine("*               My Server Work             *");
    Console.WriteLine("********************************************");
    Console.ResetColor();
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

app.MapPost("/register", async (User user, [FromServices] ToDoDbContext dbContext) =>
{
    dbContext.Users.Add(user);
    await dbContext.SaveChangesAsync();
    return Results.Ok("User registered successfully");
});

app.MapPost("/login", async (User user, [FromServices] ToDoDbContext dbContext) =>
{
    var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == user.Username && u.Password == user.Password);
    if (existingUser == null)
        return Results.BadRequest("Invalid username or password");

    var token = GenerateJwtToken(existingUser, key);
    return Results.Ok(new { Token = token });
});

app.MapGet("/tasks", async ([FromServices] ToDoDbContext _context) =>
{
    var items = await _context.Items.ToListAsync();
    return Results.Ok(items);
});

app.MapPost("/tasks", async (Item item, [FromServices] ToDoDbContext _context) =>
{
    var itemToAdd = new Item { Name = item.Name, IsComplete = false };
    var newItem = await _context.Items.AddAsync(itemToAdd);
    await _context.SaveChangesAsync();
    return Results.Ok(newItem.Entity);
});

app.MapPut("/tasks/{id}", async (int id, [FromServices] ToDoDbContext _context) =>
{
    var itemToUpdate = await _context.Items.FindAsync(id);
    if (itemToUpdate == null)
    {
        return Results.NotFound();
    }

    itemToUpdate.IsComplete = !itemToUpdate.IsComplete;

    await _context.SaveChangesAsync();
    return Results.Ok(itemToUpdate);
});

app.MapDelete("/tasks/{id}", async (int id, [FromServices] ToDoDbContext _context) =>
{
    var item = await _context.Items.FindAsync(id);
    if (item == null)
    {
        return Results.NotFound();
    }

    _context.Items.Remove(item);
    await _context.SaveChangesAsync();
    return Results.Ok(item);
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAnyOrigin");
app.UseAuthentication();
app.UseAuthorization();

app.Run();

// Method to generate a strong key
byte[] GenerateStrongKey()
{
    using var rng = RandomNumberGenerator.Create();
    var keyBytes = new byte[32]; // 256 bits
    rng.GetBytes(keyBytes);
    return keyBytes;
}

string GenerateJwtToken(User user, byte[] key)
{
    var tokenHandler = new JwtSecurityTokenHandler();

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        }),
        Expires = DateTime.UtcNow.AddHours(1), // Token expiration time
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}

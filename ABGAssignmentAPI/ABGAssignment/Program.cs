using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ABGAssignment.ApplicationContext;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Application configuration settings

builder.Services.AddControllers();

builder.Services.AddDbContext<AbgDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AbgDbContext"));
});

// Others configuration settings (Optional)

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
});

// Add an in-memory cache

builder.Services.AddMemoryCache();

// Configure authentication using JWT tokens

var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Enable CORS (Allow requests from a specific origin)

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://127.0.0.1:5500") 
               .AllowAnyHeader()
               .AllowAnyMethod();
    });

    //options.AddPolicy("AllowAnyOrigin", builder =>
    //{
    //    builder.AllowAnyOrigin() // Allow requests from any origin
    //           .AllowAnyHeader()
    //           .AllowAnyMethod();
    //});

});

// Configure authorization policies (Optional)

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin")); 
//});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Enable CORS
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();

app.UseAuthorization();

// Configure API endpoints

app.MapControllers();

app.Run();











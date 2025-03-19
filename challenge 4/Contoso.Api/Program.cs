using Contoso.Api.Models;
using Contoso.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Contoso.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

});

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer("Bearer", options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
    };
});

builder.Services.AddAutoMapper(typeof(MapperConfig));

// Get connection string for SQL Server
// var connectionString = builder.Configuration.GetConnectionString("ContosoDBConnection");

var cosmosConnectionString = builder.Configuration["Azure:CosmosDB:ConnectionString"];
var cosmosDatabaseName = builder.Configuration["Azure:CosmosDB:DatabaseName"];

Console.WriteLine($"Cosmos Connection String: {cosmosConnectionString}");
Console.WriteLine($"Cosmos Database Name: {cosmosDatabaseName}");

// Add DbContext
builder.Services.AddDbContext<ContosoDbContext>(options =>
{
    // options.UseSqlServer(connectionString);
    options.UseCosmos(
        cosmosConnectionString,
        cosmosDatabaseName
    );
}, ServiceLifetime.Scoped);


builder.Services.AddTransient<IProductsService, ProductsService>();
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddSingleton<IProductImageService, ProductImageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

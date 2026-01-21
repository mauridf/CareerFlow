using System.Text;
using CareerFlow.API.Extensions;
using CareerFlow.API.Middleware;
using CareerFlow.Domain.Common;
using CareerFlow.Infrastructure;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

// Carregar .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog para logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting CareerFlow API");

    // Add services to the container
    builder.Services.AddControllers();

    // Add Infrastructure
    builder.Services.AddInfrastructure(builder.Configuration);

    // Custom configurations
    builder.Services.AddSwaggerDocumentation();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Aplicar migrations e seed data
    await app.ApplyMigrationsAndSeedAsync();


    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "CareerFlow API v1");
            c.RoutePrefix = "swagger";
        });
    }

    // Configurar JWT
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    builder.Services.Configure<JwtSettings>(jwtSettings);

    var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization();

    app.UseHttpsRedirection();

    // Custom middleware
    app.UseMiddleware<ExceptionMiddleware>();

    app.UseAuthorization();

    app.MapControllers();

    // Health check endpoint
    app.MapGet("/health", () => "CareerFlow API is running!");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
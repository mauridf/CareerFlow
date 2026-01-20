using CareerFlow.API.Extensions;
using CareerFlow.API.Middleware;
using Serilog;
using DotNetEnv;

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

    // Custom configurations
    builder.Services.AddSwaggerDocumentation();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

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
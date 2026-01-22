using System.Text;
using CareerFlow.API.Extensions;
using CareerFlow.API.Middleware;
using CareerFlow.Domain.Common;
using CareerFlow.Infrastructure;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using DotNetEnv;

// Carregar variáveis de ambiente
Env.Load();

var builder = WebApplication.CreateBuilder(args);


// Configurar para Render
var isRender = Environment.GetEnvironmentVariable("RENDER") != null;
var port = Environment.GetEnvironmentVariable("PORT");

if (isRender && !string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://*:{port}");

    // Log específico para Render
    Console.WriteLine($"🚀 Render Environment Detected - Port: {port}");
}

// Se DATABASE_URL existe (formato do Render), converter para ConnectionString
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl))
{
    // Converter DATABASE_URL do Render para ConnectionString do PostgreSQL
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');

    var connectionString = $"Host={uri.Host};" +
                          $"Port={uri.Port};" +
                          $"Database={uri.AbsolutePath.TrimStart('/')};" +
                          $"Username={userInfo[0]};" +
                          $"Password={userInfo[1]};" +
                          $"SSL Mode=Require;" +
                          $"Trust Server Certificate=true";

    builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;
    Console.WriteLine($"✅ Database URL converted for Render");
}

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
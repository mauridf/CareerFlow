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
    Console.WriteLine($"🚀 Render Environment Detected - Port: {port}");
}

// SEÇÃO CRÍTICA: Converter DATABASE_URL
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl))
{
    try
    {
        Console.WriteLine($"📦 DATABASE_URL encontrada: {databaseUrl.Substring(0, Math.Min(databaseUrl.Length, 50))}...");

        // CORREÇÃO 1: Se não tem :5432, adiciona antes do /
        if (!databaseUrl.Contains(":5432") && !databaseUrl.Contains(":"))
        {
            // Encontra a posição do @ e do /
            var atIndex = databaseUrl.IndexOf('@');
            var slashIndex = databaseUrl.IndexOf('/', atIndex);

            if (atIndex > 0 && slashIndex > 0)
            {
                databaseUrl = databaseUrl.Insert(slashIndex, ":5432");
                Console.WriteLine($"🔧 Porta 5432 adicionada: {databaseUrl}");
            }
        }

        var uri = new Uri(databaseUrl);

        // CORREÇÃO 2: Verificar se porta é válida
        var portNumber = uri.Port > 0 ? uri.Port : 5432;

        // CORREÇÃO 3: Verificar hostname
        var host = uri.Host;
        if (!host.Contains("."))
        {
            host += ".render.com"; // Adiciona sufixo se necessário
            Console.WriteLine($"🌐 Hostname ajustado: {host}");
        }

        var database = uri.AbsolutePath.Trim('/');
        var userInfo = uri.UserInfo.Split(':');

        if (userInfo.Length != 2)
        {
            throw new FormatException("Formato de usuário/senha inválido na DATABASE_URL");
        }

        var connectionString = $"Host={host};" +
                              $"Port={portNumber};" +
                              $"Database={database};" +
                              $"Username={userInfo[0]};" +
                              $"Password={userInfo[1]};" +
                              $"SSL Mode=Require;" +
                              $"Trust Server Certificate=true";

        builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;
        Console.WriteLine($"✅ Database configurado via DATABASE_URL");
        Console.WriteLine($"📊 Host: {host}, Database: {database}, User: {userInfo[0]}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ ERRO ao processar DATABASE_URL: {ex.Message}");
        Console.WriteLine($"🔍 URL: {databaseUrl}");

        // Fallback: usar appsettings.json
        var fallbackConnection = builder.Configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(fallbackConnection))
        {
            Console.WriteLine($"🔄 Usando connection string do appsettings.json");
        }
    }
}
else
{
    Console.WriteLine("ℹ️ DATABASE_URL não encontrada, usando appsettings.json");
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

    app.UseHttpsRedirection();

    // Custom middleware
    app.UseMiddleware<ExceptionMiddleware>();

    app.UseAuthentication();
    
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
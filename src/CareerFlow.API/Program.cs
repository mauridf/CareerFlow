using System.Text;
using CareerFlow.API.Extensions;
using CareerFlow.API.Middleware;
using CareerFlow.Domain.Common;
using CareerFlow.Infrastructure;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

// Carregar variáveis de ambiente do .env (se existir)
if (File.Exists(".env"))
{
    Env.Load();
}
else
{
    Console.WriteLine("ℹ️ Arquivo .env não encontrado, usando variáveis de ambiente do sistema");
}

var builder = WebApplication.CreateBuilder(args);

// Configurar para Render APENAS se detectado
var isRender = Environment.GetEnvironmentVariable("RENDER") != null;
var port = Environment.GetEnvironmentVariable("PORT");

if (isRender && !string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://*:{port}");
    Console.WriteLine($"🚀 Render Environment Detected - Port: {port}");

    // NO RENDER: Processar DATABASE_URL
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrEmpty(databaseUrl))
    {
        try
        {
            Console.WriteLine($"📦 Processando DATABASE_URL do Render...");

            // Parse da URL do Render
            var uri = new Uri(databaseUrl);
            var host = uri.Host;

            // Adicionar domínio completo se necessário (Render específico)
            if (host.StartsWith("dpg-") && !host.Contains("."))
            {
                host += ".oregon-postgres.render.com"; // Ajuste para sua região
                Console.WriteLine($"🌐 Hostname do Render ajustado: {host}");
            }

            var portNumber = uri.Port > 0 ? uri.Port : 5432;
            var database = uri.AbsolutePath.Trim('/');
            var userInfo = uri.UserInfo.Split(':');

            if (userInfo.Length != 2)
                throw new FormatException("Formato de credenciais inválido");

            var connectionString = $"Host={host};" +
                                  $"Port={portNumber};" +
                                  $"Database={database};" +
                                  $"Username={userInfo[0]};" +
                                  $"Password={userInfo[1]};" +
                                  $"SSL Mode=Require;" +
                                  $"Trust Server Certificate=true";

            builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;
            Console.WriteLine($"✅ Database do Render configurado");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERRO ao processar DATABASE_URL: {ex.Message}");
            throw; // Falha crítica no Render
        }
    }
}
else
{
    // LOCAL: Usar appsettings.*.json normalmente
    Console.WriteLine($"🏠 Ambiente Local - Portas: 5249 (HTTP), 7051 (HTTPS)");

    // Opcional: Log da connection string local (sem senha)
    var localConnection = builder.Configuration.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrEmpty(localConnection))
    {
        var safeLog = localConnection.Contains("Password=")
            ? localConnection.Substring(0, localConnection.IndexOf("Password=")) + "Password=***"
            : localConnection;
        Console.WriteLine($"🔗 Connection String local: {safeLog}");
    }
}

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting CareerFlow API - Environment: {Environment}",
        builder.Environment.EnvironmentName);

    // Add services to the container
    builder.Services.AddControllers();

    // Add Infrastructure
    builder.Services.AddInfrastructure(builder.Configuration);

    // Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerDocumentation(); // Seu método personalizado

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

    // Aplicar migrations e seed data APENAS se configurado
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrEmpty(connectionString) && !connectionString.Contains("${DATABASE_URL}"))
    {
        await app.ApplyMigrationsAndSeedAsync();
    }
    else
    {
        Console.WriteLine("⚠️  Database não configurado, pulando migrations");
    }

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "CareerFlow API v1");
            c.RoutePrefix = "swagger";
        });

        // Adicionar CORS para desenvolvimento local
        app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
    }
    else
    {
        // Em produção, Swagger pode ser protegido ou desabilitado
        app.UseSwagger();
        // app.UseSwaggerUI(); // Descomente se quiser Swagger em produção
    }

    app.UseHttpsRedirection();
    app.UseMiddleware<ExceptionMiddleware>();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.MapGet("/health", () => new {
        Status = "Healthy",
        Environment = app.Environment.EnvironmentName,
        Timestamp = DateTime.UtcNow,
        Database = !string.IsNullOrEmpty(connectionString) ? "Configured" : "Not configured"
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
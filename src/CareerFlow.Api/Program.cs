using System.Text;
using System.Text.Json;
using CareerFlow.Api.Extensions;
using CareerFlow.Api.Middlewares;
using CareerFlow.Api.Services;
using CareerFlow.Application;
using CareerFlow.Application.Common.DTOs;
using CareerFlow.Core.Interfaces;
using CareerFlow.Core.Interfaces.Settings;
using CareerFlow.Infrastructure;
using CareerFlow.Infrastructure.Data;
using CareerFlow.Infrastructure.Data.Migrations;
using CareerFlow.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;

// ============================================
// Configuração inicial do Serilog (Bootstrap)
// Captura erros antes do app estar pronto
// ============================================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateBootstrapLogger();

Log.Information("🚀 Iniciando CareerFlow API...");
Log.Information("📅 Data/Hora: {DateTime}", DateTimeOffset.Now);

try
{
    // Configura Npgsql para aceitar DateTime com Kind=Unspecified
    // (necessário pois o frontend envia datas sem timezone como "2024-01-01")
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    var builder = WebApplication.CreateBuilder(args);

    // ============================================
    // Configuração SERILOG (Completa)
    // ============================================
    builder.Host.UseSerilog((context, services, configuration) =>
    {
        // Obtém o HttpContextAccessor do serviço para os enrichers
        var httpContextAccessor = services.GetRequiredService<IHttpContextAccessor>();

        configuration
            // Níveis mínimos
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)

            // Em desenvolvimento, mostra queries SQL
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command",
                context.HostingEnvironment.IsDevelopment() ? LogEventLevel.Information : LogEventLevel.Warning)

            // Lê configurações adicionais do appsettings
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)

            // Enriquecimento
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .Enrich.WithCorrelationId()
            .Enrich.WithClientIp(httpContextAccessor)      // Com DI
            .Enrich.WithClientAgent(httpContextAccessor)    // Com DI

            // Destruturação de objetos
            .Destructure.ToMaximumDepth(4)
            .Destructure.ToMaximumStringLength(1000)
            .Destructure.ToMaximumCollectionCount(20)
            .Destructure.WithSensitiveDataMasking()

            // Sinks
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj} {Properties:j}{NewLine}{Exception}",
                restrictedToMinimumLevel: context.HostingEnvironment.IsDevelopment()
                    ? LogEventLevel.Debug
                    : LogEventLevel.Information)

            .WriteTo.File(
                path: "logs/careerflow-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Information)

            // Em produção, adiciona log JSON para ferramentas de análise
            .WriteTo.Conditional(
                condition: _ => context.HostingEnvironment.IsProduction(),
                configureSink: sinkConfiguration =>
                {
                    sinkConfiguration.File(
                        path: "logs/careerflow-json-.log",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 14,
                        formatter: new Serilog.Formatting.Json.JsonFormatter(),
                        restrictedToMinimumLevel: LogEventLevel.Warning);
                });
    });

    // ============================================
    // Registrar HttpContextAccessor (necessário para enrichers)
    // ============================================
    builder.Services.AddHttpContextAccessor();

    // ============================================
    // Configurações fortemente tipadas
    // ============================================
    builder.Services.AddAppSettings(builder.Configuration);

    // ============================================
    // JWT Authentication
    // ============================================
    var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;
    var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = "role",
            NameClaimType = "given_name"
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Log.Warning("🔒 Falha na autenticação: {Error}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Log.Debug("✅ Token validado para: {User}",
                    context.Principal?.FindFirst("email")?.Value);
                return Task.CompletedTask;
            }
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("Premium", policy =>
            policy.RequireClaim("isPremium", "true"));

        options.AddPolicy("Admin", policy =>
            policy.RequireRole("Admin"));
    });

    // ============================================
    // Rate Limiting
    // ============================================
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        options.OnRejected = async (context, cancellationToken) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.HttpContext.Response.ContentType = "application/json";

            var retryAfter = context.Lease.TryGetMetadata(
                System.Threading.RateLimiting.MetadataName.RetryAfter, out var retryAfterValue)
                ? retryAfterValue.TotalSeconds.ToString()
                : "60";

            var errorResponse = new
            {
                success = false,
                error = new
                {
                    code = "RATE_LIMIT",
                    message = "Muitas requisições. Aguarde um momento e tente novamente."
                },
                meta = new
                {
                    timestamp = DateTime.UtcNow,
                    requestId = context.HttpContext.TraceIdentifier,
                    retryAfter = retryAfter
                }
            };

            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.HttpContext.Response.WriteAsync(json, cancellationToken);
        };

        // Login: 5 tentativas por minuto
        options.AddFixedWindowLimiter("Login", config =>
        {
            config.PermitLimit = 5;
            config.Window = TimeSpan.FromMinutes(1);
            config.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
            config.QueueLimit = 0;
        });

        // Register: 3 por hora
        options.AddFixedWindowLimiter("Register", config =>
        {
            config.PermitLimit = 3;
            config.Window = TimeSpan.FromHours(1);
        });

        // Global: 100 por minuto
        options.AddFixedWindowLimiter("Global", config =>
        {
            config.PermitLimit = 100;
            config.Window = TimeSpan.FromMinutes(1);
        });
    });

    // ============================================
    // Registrar serviços da aplicação
    // ============================================
    builder.Services.AddApplicationServices();

    // ============================================
    // Registrar serviços das camadas
    // ============================================
    builder.Services.AddApplication();      // MediatR, FluentValidation, AutoMapper
    builder.Services.AddInfrastructure(builder.Configuration); // EF Core, Repos, Outbox

    // ============================================
    // Portas e URLs
    // ============================================
    builder.WebHost.UseUrls("http://localhost:5000");
    builder.WebHost.UseShutdownTimeout(TimeSpan.FromSeconds(10));

    // ============================================
    // Serviços da aplicação
    // ============================================
    builder.Services.AddControllers(options =>
    {
        // Retorna 406 se o formato não for suportado
        options.ReturnHttpNotAcceptable = true;
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddOpenApi();

    // ============================================
    // Health Checks
    // ============================================
    builder.Services.AddHealthChecks();

    // ============================================
    // CORS
    // ============================================
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            var corsOrigins = builder.Configuration
                .GetSection("Application:CorsOrigins").Get<string[]>()
                ?? new[] { "http://localhost:3000" };

            policy.WithOrigins(corsOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    // ============================================
    // Serviços da API
    // ============================================
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddScoped<PersonRepository>();

    // ============================================
    // URLs
    // ============================================
    builder.WebHost.UseUrls("http://localhost:5000");

    // ============================================
    // Configurar servir arquivos estáticos da pasta uploads
    // ============================================
    builder.Services.AddDirectoryBrowser();

    var app = builder.Build();

    // ============================================
    // Iniciar OutboxProcessor após o app iniciar
    // ============================================
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        var outboxProcessor = app.Services.GetRequiredService<CareerFlow.Infrastructure.Outbox.OutboxProcessor>();
        outboxProcessor.Start();
    });

    app.Lifetime.ApplicationStopping.Register(() =>
    {
        var outboxProcessor = app.Services.GetRequiredService<CareerFlow.Infrastructure.Outbox.OutboxProcessor>();
        outboxProcessor.Stop();
    });

    // ============================================
    // Executar Migrations e Seed (Desenvolvimento)
    // ============================================
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CareerFlowDbContext>();

        try
        {
            Log.Information("📊 Aplicando migrations...");
            await dbContext.Database.EnsureCreatedAsync();
            Log.Information("✅ Banco de dados verificado");

            Log.Information("🌱 Aplicando dados iniciais...");
            await SeedData.SeedAsync(dbContext);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "❌ Erro ao aplicar migrations/seed");
        }
    }

    // ============================================
    // Pipeline de Middleware
    // ============================================

    app.UseGlobalExceptionHandler();
    // 1. Serilog Request Logging (primeiro para capturar tudo)
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} respondeu {StatusCode} em {Elapsed:0.0000}ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString());
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
            diagnosticContext.Set("CorrelationId", httpContext.TraceIdentifier);
        };
    });

    // 2. Request Logging customizado
    app.UseRequestLogging();

    // 3. CORS
    app.UseCors();

    app.UseRateLimiter();

    // 4. Arquivos estáticos (uploads) - antes do roteamento para servir sem auth
    app.UseStaticFiles(); // wwwroot padrão (se houver)

    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
    if (Directory.Exists(uploadsPath))
    {
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
            RequestPath = "/uploads"
        });
    }

    // 5. Roteamento
    app.UseRouting();

    // 6. Authentication e Authorization
    app.UseAuthentication();
    app.UseAuthorization();

    // 7. Endpoints
    app.MapControllers();
    app.MapHealthChecks("/health");

    // Documentação (apenas desenvolvimento)
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options
                .WithTitle("CareerFlow API")
                .WithTheme(ScalarTheme.Purple)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                .WithDarkModeToggle(true);
        });
    }

    // ============================================
    // Outbox Processor
    // ============================================
    var outboxProcessor = app.Services.GetRequiredService<CareerFlow.Infrastructure.Outbox.OutboxProcessor>();
    outboxProcessor.Start();

    // ============================================
    // Eventos de ciclo de vida
    // ============================================
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        var url = app.Configuration["Application:Url"] ?? "http://localhost:5000";
        Log.Information("✅ CareerFlow API iniciada com sucesso em {Url}", url);
        Log.Information("📊 Health check: {Url}/health", url);

        if (app.Environment.IsDevelopment())
        {
            Log.Information("📚 Documentação Scalar: {Url}/scalar/v1", url);
            Log.Information("📚 Documentação OpenAPI: {Url}/openapi/v1.json", url);
        }

        Log.Information("📚 Ambiente: {Environment}", app.Environment.EnvironmentName);
    });

    app.Lifetime.ApplicationStopping.Register(() =>
    {
        Log.Information("🛑 CareerFlow API está parando...");
    });

    app.Lifetime.ApplicationStopped.Register(() =>
    {
        Log.Information("⏹️ CareerFlow API parada.");
    });

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Falha catastrófica ao iniciar a aplicação");
    throw;
}
finally
{
    Log.Information("🏁 Aplicação finalizada.");
    await Log.CloseAndFlushAsync();
}

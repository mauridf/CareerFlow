using CareerFlow.Api.Extensions;
using CareerFlow.Api.Middlewares;
using Microsoft.EntityFrameworkCore;
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
    // Registrar serviços da aplicação
    // ============================================
    builder.Services.AddApplicationServices();

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
                .GetSection("Application:CorsOrigins")
                .Get<string[]>() ?? new[] { "http://localhost:3000" };

            policy.WithOrigins(corsOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    var app = builder.Build();

    // ============================================
    // Executar Migrations e Seed (Desenvolvimento)
    // ============================================
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CareerFlow.Infrastructure.Data.CareerFlowDbContext>();

        try
        {
            Log.Information("📊 Aplicando migrations...");
            await dbContext.Database.MigrateAsync();
            Log.Information("✅ Migrations aplicadas com sucesso");

            Log.Information("🌱 Aplicando dados iniciais (seed)...");
            await CareerFlow.Infrastructure.Data.Migrations.SeedData.SeedAsync(dbContext);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "❌ Erro ao aplicar migrations/seed");
            // Não impede a aplicação de iniciar
        }
    }

    // ============================================
    // Pipeline de Middleware
    // ============================================

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

    // 4. Middlewares customizados (serão adicionados nos próximos passos)
    // app.UseMiddleware<GlobalExceptionMiddleware>();

    // 5. Roteamento
    app.UseRouting();

    // 6. Authentication e Authorization (serão adicionados no passo de Auth)
    // app.UseAuthentication();
    // app.UseAuthorization();

    // 7. Endpoints
    app.MapControllers();
    app.MapHealthChecks("/health");

    // Documentação (apenas desenvolvimento)
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();

        // Configuração do Scalar
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

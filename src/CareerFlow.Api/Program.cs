using CareerFlow.Api.Extensions;
using CareerFlow.Api.Middlewares;
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
            .Enrich.WithClientIp()
            .Enrich.WithClientAgent()

            // Destruturação de objetos
            .Destructure.ToMaximumDepth(4)
            .Destructure.ToMaximumStringLength(1000)
            .Destructure.ToMaximumCollectionCount(20)

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
            policy.WithOrigins(
                    builder.Configuration.GetSection("Application:CorsOrigins").Get<string[]>() ??
                    new[] { "http://localhost:3000" })
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    var app = builder.Build();

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

    // 2. CORS
    app.UseCors();

    // 3. Middlewares customizados (serão adicionados nos próximos passos)
    // app.UseMiddleware<GlobalExceptionMiddleware>();
    // app.UseMiddleware<RequestLoggingMiddleware>();

    // 4. Roteamento
    app.UseRouting();

    // 5. Authentication e Authorization (serão adicionados no passo de Auth)
    // app.UseAuthentication();
    // app.UseAuthorization();

    // 6. Endpoints
    app.MapControllers();
    app.MapHealthChecks("/health");

    // Documentação (apenas desenvolvimento)
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalar();
    }

    // ============================================
    // Eventos de ciclo de vida
    // ============================================
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        var url = app.Configuration["Application:Url"] ?? "http://localhost:5000";
        Log.Information("✅ CareerFlow API iniciada com sucesso em {Url}", url);
        Log.Information("📊 Health check: {Url}/health", url);
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

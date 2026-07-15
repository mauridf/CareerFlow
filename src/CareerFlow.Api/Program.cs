using CareerFlow.Api.Extensions;
using Serilog;

// ============================================
// Configuração inicial do Serilog
// ============================================
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("🚀 Iniciando CareerFlow API...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ============================================
    // Configuração do Serilog (completo)
    // ============================================
    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithCorrelationId();
    });

    // ============================================
    // Configurações fortemente tipadas
    // ============================================
    builder.Services.AddAppSettings(builder.Configuration);

    // ============================================
    // Configuração das portas
    // ============================================
    builder.WebHost.UseUrls("http://localhost:5000");

    // ============================================
    // Serviços básicos
    // ============================================
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddOpenApi();

    // Health Checks
    builder.Services.AddHealthChecks();

    var app = builder.Build();

    // ============================================
    // Pipeline de Middleware
    // ============================================
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseDeveloperExceptionPage();
    }

    app.UseSerilogRequestLogging();
    app.UseRouting();
    app.MapControllers();
    app.MapHealthChecks("/health");

    // ============================================
    // Inicialização
    // ============================================
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        Log.Information("✅ CareerFlow API iniciada em {Url}",
            app.Configuration["Application:Url"]);
        Log.Information("📊 Health check: http://localhost:5000/health");
    });

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Falha ao iniciar a aplicação");
}
finally
{
    Log.CloseAndFlush();
}

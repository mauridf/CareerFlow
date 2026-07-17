using Quartz;
using Serilog;
using CareerFlow.Infrastructure;
using CareerFlow.Application;
using CareerFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using CareerFlow.Scheduler.Jobs;
using Microsoft.Extensions.Configuration;

// ============================================
// Configuração do Serilog
// ============================================
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/scheduler-.log", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

Log.Information("⏰ Iniciando CareerFlow Scheduler...");

try
{
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) =>
        {
            // ============================================
            // Configuração
            // ============================================
            var configuration = context.Configuration;

            // Connection string (do appsettings ou variável de ambiente)
            var connectionString = configuration.GetValue<string>("Database:ConnectionString")
                ?? "Host=localhost;Port=5432;Database=careerflow;Username=postgres;Password=postgres;";

            // ============================================
            // DbContext
            // ============================================
            services.AddDbContext<CareerFlowDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            // ============================================
            // Serviços
            // ============================================
            services.AddApplication();
            services.AddInfrastructure(configuration);

            // ============================================
            // Quartz.NET
            // ============================================
            services.AddQuartz(q =>
            {
                // Configura o scheduler
                q.SchedulerName = "CareerFlow Scheduler";
                q.SchedulerId = "AUTO";
                q.InterruptJobsOnShutdown = true;

                // ============================================
                // Job 1: ResumeAnalyticsJob - Diário às 02:00
                // ============================================
                var resumeAnalyticsJobKey = new JobKey("ResumeAnalyticsJob");
                q.AddJob<ResumeAnalyticsJob>(opts => opts.WithIdentity(resumeAnalyticsJobKey));
                q.AddTrigger(opts => opts
                    .ForJob(resumeAnalyticsJobKey)
                    .WithIdentity("ResumeAnalyticsTrigger")
                    .WithCronSchedule("0 0 2 * * ?", x => x
                        .WithMisfireHandlingInstructionFireAndProceed())
                    .WithDescription("Atualiza analytics do currículo - Diário 02:00"));

                // ============================================
                // Job 2: ProfileCompletionReminderJob - Semanal às 08:00
                // ============================================
                var profileReminderJobKey = new JobKey("ProfileCompletionReminderJob");
                q.AddJob<ProfileCompletionReminderJob>(opts => opts.WithIdentity(profileReminderJobKey));
                q.AddTrigger(opts => opts
                    .ForJob(profileReminderJobKey)
                    .WithIdentity("ProfileCompletionReminderTrigger")
                    .WithCronSchedule("0 0 8 ? * MON", x => x
                        .WithMisfireHandlingInstructionFireAndProceed())
                    .WithDescription("Lembrete para completar perfil - Segunda 08:00"));

                // ============================================
                // Job 3: InactiveUserNotificationJob - Mensal às 09:00
                // ============================================
                var inactiveUserJobKey = new JobKey("InactiveUserNotificationJob");
                q.AddJob<InactiveUserNotificationJob>(opts => opts.WithIdentity(inactiveUserJobKey));
                q.AddTrigger(opts => opts
                    .ForJob(inactiveUserJobKey)
                    .WithIdentity("InactiveUserNotificationTrigger")
                    .WithCronSchedule("0 0 9 1 * ?", x => x
                        .WithMisfireHandlingInstructionFireAndProceed())
                    .WithDescription("Notificação para usuários inativos - Dia 1 às 09:00"));

                // ============================================
                // Job 4: CleanupJob - Semanal Domingo às 03:00
                // ============================================
                var cleanupJobKey = new JobKey("CleanupJob");
                q.AddJob<CleanupJob>(opts => opts.WithIdentity(cleanupJobKey));
                q.AddTrigger(opts => opts
                    .ForJob(cleanupJobKey)
                    .WithIdentity("CleanupTrigger")
                    .WithCronSchedule("0 0 3 ? * SUN", x => x
                        .WithMisfireHandlingInstructionFireAndProceed())
                    .WithDescription("Limpeza de dados antigos - Domingo 03:00"));

                // ============================================
                // Job 5: PremiumExpirationJob - Diário às 04:00
                // ============================================
                var premiumExpirationJobKey = new JobKey("PremiumExpirationJob");
                q.AddJob<PremiumExpirationJob>(opts => opts.WithIdentity(premiumExpirationJobKey));
                q.AddTrigger(opts => opts
                    .ForJob(premiumExpirationJobKey)
                    .WithIdentity("PremiumExpirationTrigger")
                    .WithCronSchedule("0 0 4 * * ?", x => x
                        .WithMisfireHandlingInstructionFireAndProceed())
                    .WithDescription("Verifica expiração de premium - Diário 04:00"));

                // ============================================
                // Job 6: ResumeSuggestionJob - Diário às 05:00
                // ============================================
                var suggestionJobKey = new JobKey("ResumeSuggestionJob");
                q.AddJob<ResumeSuggestionJob>(opts => opts.WithIdentity(suggestionJobKey));
                q.AddTrigger(opts => opts
                    .ForJob(suggestionJobKey)
                    .WithIdentity("ResumeSuggestionTrigger")
                    .WithCronSchedule("0 0 5 * * ?", x => x
                        .WithMisfireHandlingInstructionFireAndProceed())
                    .WithDescription("Gera sugestões de melhoria - Diário 05:00"));
            });

            // Adiciona o serviço hospedado do Quartz
            services.AddQuartzHostedService(q =>
            {
                q.WaitForJobsToComplete = true;
                q.AwaitApplicationStarted = true;
            });
        })
        .UseSerilog()
        .Build();

    // ============================================
    // Inicialização
    // ============================================
    var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

    lifetime.ApplicationStarted.Register(() =>
    {
        Log.Information("✅ CareerFlow Scheduler iniciado com sucesso");
        Log.Information("📋 Jobs configurados:");
        Log.Information("   • ResumeAnalyticsJob        - Diário 02:00");
        Log.Information("   • ProfileCompletionReminder  - Segunda 08:00");
        Log.Information("   • InactiveUserNotification   - Dia 1 09:00");
        Log.Information("   • CleanupJob                - Domingo 03:00");
        Log.Information("   • PremiumExpirationJob      - Diário 04:00");
        Log.Information("   • ResumeSuggestionJob       - Diário 05:00");
    });

    lifetime.ApplicationStopping.Register(() =>
    {
        Log.Information("🛑 CareerFlow Scheduler parando...");
    });

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Falha ao iniciar o Scheduler");
}
finally
{
    await Log.CloseAndFlushAsync();
}

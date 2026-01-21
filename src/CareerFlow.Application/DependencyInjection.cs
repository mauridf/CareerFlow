using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CareerFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registrar AutoMapper
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        // Registrar FluentValidation
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Registrar serviços (serão implementados no próximo passo)
        // services.AddScoped<IUserService, UserService>();
        // services.AddScoped<ISkillService, SkillService>();
        // etc...

        return services;
    }
}
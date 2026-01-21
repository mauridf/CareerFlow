using CareerFlow.Application.Common;
using CareerFlow.Application.Interfaces;
using CareerFlow.Application.Services;
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

        // Registrar serviços
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<IATSResumeService, ATSResumeService>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<ISkillService, SkillService>();
        services.AddScoped<IExperienceService, ExperienceService>();
        services.AddScoped<IAcademicService, AcademicService>();
        services.AddScoped<ICertificateService, CertificateService>();
        services.AddScoped<ILanguageService, LanguageService>();

        return services;
    }
}
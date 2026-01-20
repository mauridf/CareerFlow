using Microsoft.OpenApi;

namespace CareerFlow.API.Extensions;

public static class SwaggerServiceExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CareerFlow API",
                Version = "v1",
                Description = "API for professional career management and CV/Resume building",
                Contact = new OpenApiContact
                {
                    Name = "Maurício Carvalho Developer",
                    Email = "mauricio.carvalho.developer@gmail.com"
                }
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Informe o token JWT no formato: Bearer {token}"
            });

            // JEITO CERTO NO .NET 10 + Swashbuckle 10.x
            c.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer"),
                    new List<string>()
                }
            });
        });

        return services;
    }
}

using System.Reflection;
using Microsoft.OpenApi;

namespace LojaTintas.API.Extensions;

/// <summary>Configuração centralizada do Swagger/OpenAPI (CP3).</summary>
public static class SwaggerServiceExtensions
{
    public static IServiceCollection AddLojaTintasSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "LojaTintas API",
                Version = "v1",
                Description = "API REST para gerenciamento de uma loja de tintas: produtos, estoque, " +
                              "pedidos e relacionamento com fornecedores e fabricantes."
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        });

        return services;
    }
}

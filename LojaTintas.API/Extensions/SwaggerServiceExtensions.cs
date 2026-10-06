using System.Reflection;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LojaTintas.API.Extensions;

/// <summary>Configuração centralizada do Swagger/OpenAPI (CP3).</summary>
public static class SwaggerServiceExtensions
{
    public static IServiceCollection AddLojaTintasSwagger(this IServiceCollection services)
    {
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        });

        return services;
    }
}

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfo(description));
        }
    }

    private static OpenApiInfo CreateInfo(ApiVersionDescription description)
    {
        var text = "API REST para gerenciamento de uma loja de tintas: produtos, estoque, " +
                   "pedidos e relacionamento com fornecedores e fabricantes.";

        if (description.IsDeprecated)
        {
            text += " ⚠️ ESTA VERSÃO ESTÁ DEPRECADA: continua funcionando, mas será desligada. " +
                    "A listagem de pedidos desta versão devolve o array completo, sem paginação. " +
                    "Migre para a versão 2.0.";
        }
        else
        {
            text += " Versão atual: a listagem de pedidos é paginada (page, pageSize) e devolve um envelope com totais.";
        }

        return new OpenApiInfo
        {
            Title = "LojaTintas API",
            Version = description.ApiVersion.ToString(),
            Description = text
        };
    }
}

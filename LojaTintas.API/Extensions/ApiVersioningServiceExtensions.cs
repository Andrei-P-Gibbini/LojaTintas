using Asp.Versioning;

namespace LojaTintas.API.Extensions;

public static class ApiVersioningServiceExtensions
{
    public static IServiceCollection AddLojaTintasApiVersioning(this IServiceCollection services)
    {
        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(2, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;

                options.ReportApiVersions = true;

                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("X-Api-Version"));
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVVV";
                options.AssumeDefaultVersionWhenUnspecified = true;
            });

        return services;
    }
}

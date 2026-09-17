using LojaTintas.API.HealthChecks;
using LojaTintas.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LojaTintas.API.Extensions;

/// <summary>Configuração centralizada dos health checks (CP4).</summary>
public static class HealthCheckServiceExtensions
{
    public static IServiceCollection AddLojaTintasHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            // Processo no ar — não depende de nenhuma infraestrutura externa.
            .AddCheck("self", () => HealthCheckResult.Healthy("API em execução."), tags: ["self"])
            // Banco usado no CP2 (SQLite via LojaTintasDbContext), alinhado ao DbContext do EF Core.
            .AddDbContextCheck<LojaTintasDbContext>(
                name: "database",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["database"])
            // Dependência externa de exemplo (item recomendado do CP4).
            .AddCheck<FiapWebsiteHealthCheck>("fiap-website", tags: ["external"]);

        return services;
    }
}

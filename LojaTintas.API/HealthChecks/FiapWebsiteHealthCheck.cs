using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LojaTintas.API.HealthChecks;

/// <summary>
/// Check recomendado (CP4, item opcional): verifica uma dependência externa de exemplo
/// (site da FIAP). Se essa URL cair, o relatório agregado de /health também fica
/// Unhealthy (503) — comportamento intencional: um check externo Unhealthy derruba
/// o status geral, já que /health reporta a saúde de todas as dependências registradas.
/// </summary>
public class FiapWebsiteHealthCheck : IHealthCheck
{
    private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(5) };

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await HttpClient.GetAsync("https://www.fiap.com.br", cancellationToken);

            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy($"FIAP respondeu {(int)response.StatusCode}.")
                : HealthCheckResult.Degraded($"FIAP respondeu {(int)response.StatusCode}.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Não foi possível acessar o site da FIAP.", ex);
        }
    }
}

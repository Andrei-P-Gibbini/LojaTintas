using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LojaTintas.API.HealthChecks;

/// <summary>
/// Formata o relatório de /health como JSON legível (CP4): status geral, duração total,
/// traceId da requisição e a lista de checks individuais (nome, status, duração).
/// O detalhe da exceção só é incluído em ambiente de Development.
/// </summary>
public static class HealthCheckResponseWriter
{
    public static Task WriteAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var isDevelopment = context.RequestServices
            .GetRequiredService<IHostEnvironment>()
            .IsDevelopment();

        var payload = new
        {
            status = report.Status.ToString(),
            totalDurationMs = Math.Round(report.TotalDuration.TotalMilliseconds, 2),
            traceId = context.TraceIdentifier,
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                durationMs = Math.Round(entry.Value.Duration.TotalMilliseconds, 2),
                description = entry.Value.Description,
                error = isDevelopment ? entry.Value.Exception?.Message : null
            })
        };

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        return context.Response.WriteAsync(json);
    }
}

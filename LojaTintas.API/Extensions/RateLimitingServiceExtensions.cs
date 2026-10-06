using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LojaTintas.API.Extensions;

public static class RateLimitingServiceExtensions
{
    public const string EscritaPolicy = "escrita";

    public const string LeituraPolicy = "leitura";

    private const int EscritaPermitLimit = 10;
    private const int LeituraPermitLimit = 100;
    private static readonly TimeSpan Janela = TimeSpan.FromMinutes(1);

    public static IServiceCollection AddLojaTintasRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy(EscritaPolicy, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: GetClientKey(httpContext),
                    factory: _ => CriarOpcoes(EscritaPermitLimit)));

            options.AddPolicy(LeituraPolicy, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: GetClientKey(httpContext),
                    factory: _ => CriarOpcoes(LeituraPermitLimit)));

            options.OnRejected = async (context, cancellationToken) =>
            {
                var httpContext = context.HttpContext;

                var retryAfterSeconds = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
                    ? (int)Math.Ceiling(retryAfter.TotalSeconds)
                    : (int)Janela.TotalSeconds;

                httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                httpContext.Response.Headers["Retry-After"] =
                    retryAfterSeconds.ToString(CultureInfo.InvariantCulture);
                httpContext.Response.ContentType = "application/problem+json";

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status429TooManyRequests,
                    Title = "Muitas requisições",
                    Detail = $"Limite de requisições excedido para este endpoint. Tente novamente em {retryAfterSeconds} segundo(s).",
                    Instance = httpContext.Request.Path,
                    Type = "https://httpstatuses.io/429"
                };
                problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

                await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            };
        });

        return services;
    }

    private static FixedWindowRateLimiterOptions CriarOpcoes(int permitLimit) => new()
    {
        PermitLimit = permitLimit,
        Window = Janela,
        QueueLimit = 0,
        AutoReplenishment = true
    };

    private static string GetClientKey(HttpContext httpContext)
        => httpContext.Connection.RemoteIpAddress?.ToString() ?? "desconhecido";
}

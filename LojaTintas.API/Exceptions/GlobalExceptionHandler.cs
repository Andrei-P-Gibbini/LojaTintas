using LojaTintas.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LojaTintas.API.Exceptions;

/// <summary>
/// Handler global de exceções (CP3/CP4). Converte qualquer exceção não tratada em uma
/// resposta ProblemDetails (RFC 7807, application/problem+json), evitando vazar
/// stack trace ou detalhes internos fora do ambiente de desenvolvimento. A exceção é
/// sempre logada em nível Error com o traceId da requisição para correlação (CP4).
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;

        _logger.LogError(
            exception,
            "Exceção não tratada. TraceId={TraceId} Path={Path} Method={Method}",
            traceId,
            httpContext.Request.Path,
            httpContext.Request.Method);

        var (status, title) = exception switch
        {
            ResourceNotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflito"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
            DomainException => (StatusCodes.Status400BadRequest, "Regra de negócio violada"),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor")
        };

        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = status == StatusCodes.Status500InternalServerError && !_environment.IsDevelopment()
                ? "Ocorreu um erro inesperado. Tente novamente mais tarde."
                : exception.Message,
            Instance = httpContext.Request.Path,
            Type = $"https://httpstatuses.io/{status}"
        };

        // Em Development, expõe o traceId na resposta para facilitar correlação com o log.
        // Em Production o traceId fica só no log — a resposta não vaza detalhe interno.
        if (_environment.IsDevelopment())
            problemDetails.Extensions["traceId"] = traceId;

        httpContext.Response.StatusCode = status;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, options: null, contentType: "application/problem+json", cancellationToken: cancellationToken);

        return true;
    }
}

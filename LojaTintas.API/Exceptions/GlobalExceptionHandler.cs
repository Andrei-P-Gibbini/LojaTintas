using LojaTintas.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LojaTintas.API.Exceptions;

/// <summary>
/// Handler global de exceções (CP3). Converte qualquer exceção não tratada em uma
/// resposta ProblemDetails (RFC 7807, application/problem+json), evitando vazar
/// stack trace ou detalhes internos fora do ambiente de desenvolvimento.
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
        _logger.LogError(exception, "Exceção não tratada em {Path}", httpContext.Request.Path);

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

        httpContext.Response.StatusCode = status;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}

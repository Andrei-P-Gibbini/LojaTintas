namespace LojaTintas.Domain.Exceptions;

/// <summary>
/// Exceção base para violações de regras de negócio do domínio.
/// Mapeada pelo GlobalExceptionHandler da API para HTTP 400 por padrão.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}

namespace LojaTintas.Domain.Exceptions;

/// <summary>
/// Lançada quando um recurso solicitado não é encontrado.
/// Mapeada pelo GlobalExceptionHandler para HTTP 404.
/// </summary>
public class ResourceNotFoundException : DomainException
{
    public ResourceNotFoundException(string resource, object id)
        : base($"{resource} com id '{id}' não foi encontrado.") { }

    public ResourceNotFoundException(string message) : base(message) { }
}

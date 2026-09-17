namespace LojaTintas.Domain.Exceptions;

/// <summary>
/// Lançada quando uma operação viola uma restrição de unicidade/estado do domínio.
/// Mapeada pelo GlobalExceptionHandler para HTTP 409.
/// </summary>
public class ConflictException : DomainException
{
    public ConflictException(string message) : base(message) { }
}

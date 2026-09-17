namespace LojaTintas.Application.DTOs.Clientes;

/// <summary>Representação de um cliente retornada pela API.</summary>
public class ClienteResponseDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CpfCnpj { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Endereco { get; set; }
    public DateTime DataCadastro { get; set; }
}

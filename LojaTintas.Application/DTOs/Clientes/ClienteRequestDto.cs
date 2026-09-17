using System.ComponentModel.DataAnnotations;

namespace LojaTintas.Application.DTOs.Clientes;

/// <summary>Dados para cadastro de um cliente.</summary>
public class ClienteRequestDto
{
    [Required, MaxLength(200)]
    public string Nome { get; set; } = string.Empty;

    [Required, MaxLength(18)]
    public string CpfCnpj { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Telefone { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Endereco { get; set; }
}

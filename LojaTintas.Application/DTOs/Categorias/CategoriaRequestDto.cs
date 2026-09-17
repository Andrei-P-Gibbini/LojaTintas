using System.ComponentModel.DataAnnotations;

namespace LojaTintas.Application.DTOs.Categorias;

/// <summary>Dados para criação/atualização de uma categoria.</summary>
public class CategoriaRequestDto
{
    [Required, MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Descricao { get; set; }
}

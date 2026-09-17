using System.ComponentModel.DataAnnotations;
using LojaTintas.Domain.Enums;

namespace LojaTintas.Application.DTOs.Produtos;

/// <summary>Dados para cadastro de um produto.</summary>
public class ProdutoRequestDto
{
    [Required, MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Descricao { get; set; }

    [Required, MaxLength(50)]
    public string CodigoSku { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal PrecoVenda { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal VolumeLitros { get; set; }

    [Required]
    public TipoTinta TipoTinta { get; set; }

    [MaxLength(80)]
    public string? CorBase { get; set; }

    [Required]
    public int CategoriaId { get; set; }

    [Required]
    public int FabricanteId { get; set; }
}

using LojaTintas.Domain.Enums;

namespace LojaTintas.Application.DTOs.Produtos;

/// <summary>Representação de um produto retornada pela API.</summary>
public class ProdutoResponseDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string CodigoSku { get; set; } = string.Empty;
    public decimal PrecoVenda { get; set; }
    public decimal VolumeLitros { get; set; }
    public TipoTinta TipoTinta { get; set; }
    public string? CorBase { get; set; }
    public int CategoriaId { get; set; }
    public string? CategoriaNome { get; set; }
    public int FabricanteId { get; set; }
    public string? FabricanteNome { get; set; }
}

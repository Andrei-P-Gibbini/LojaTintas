using LojaTintas.Domain.Enums;

namespace LojaTintas.Domain.Entities;

public class Produto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Nome { get; set; }
    public string? Descricao { get; set; }
    public required string CodigoSku { get; set; }
    public decimal PrecoVenda { get; set; }
    public decimal VolumeLitros { get; set; }
    public TipoTinta TipoTinta { get; set; }
    public string? CorBase { get; set; }

    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;

    public int FabricanteId { get; set; }
    public Fabricante Fabricante { get; set; } = null!;

    public Estoque? Estoque { get; set; }
    public ICollection<ProdutoFornecedor> ProdutoFornecedores { get; set; } = new List<ProdutoFornecedor>();
    public ICollection<ItemPedido> ItensPedido { get; set; } = new List<ItemPedido>();
}

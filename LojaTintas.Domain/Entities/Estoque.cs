namespace LojaTintas.Domain.Entities;

public class Estoque
{
    public int Id { get; set; }
    public int QuantidadeAtual { get; set; }
    public int QuantidadeMinima { get; set; }
    public required string Localizacao { get; set; }
    public DateTime UltimaAtualizacao { get; set; } = DateTime.UtcNow;

    public Guid ProdutoId { get; set; }
    public Produto Produto { get; set; } = null!;
}

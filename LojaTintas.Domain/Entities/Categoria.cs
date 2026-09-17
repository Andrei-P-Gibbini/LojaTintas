namespace LojaTintas.Domain.Entities;

public class Categoria
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public string? Descricao { get; set; }

    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
}

namespace LojaTintas.Domain.Entities;

public class Fabricante
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public required string Cnpj { get; set; }
    public string? PaisOrigem { get; set; }
    public string? Website { get; set; }

    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
}

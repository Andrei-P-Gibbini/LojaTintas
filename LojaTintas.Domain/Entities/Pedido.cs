using LojaTintas.Domain.Enums;

namespace LojaTintas.Domain.Entities;

public class Pedido
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string NumeroPedido { get; set; }
    public DateTime DataPedido { get; set; } = DateTime.UtcNow;
    public StatusPedido Status { get; set; } = StatusPedido.Aguardando;
    public decimal ValorTotal { get; set; }
    public string? Observacoes { get; set; }

    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
}

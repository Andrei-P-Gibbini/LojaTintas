using LojaTintas.Domain.Enums;

namespace LojaTintas.Application.DTOs.Pedidos;

/// <summary>Representação de um pedido retornado pela API.</summary>
public class PedidoResponseDto
{
    public Guid Id { get; set; }
    public string NumeroPedido { get; set; } = string.Empty;
    public DateTime DataPedido { get; set; }
    public StatusPedido Status { get; set; }
    public decimal ValorTotal { get; set; }
    public string? Observacoes { get; set; }
    public Guid ClienteId { get; set; }
    public string? ClienteNome { get; set; }
    public List<ItemPedidoResponseDto> Itens { get; set; } = new();
}

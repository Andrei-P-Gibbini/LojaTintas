using System.ComponentModel.DataAnnotations;

namespace LojaTintas.Application.DTOs.Pedidos;

/// <summary>Dados para criação de um pedido.</summary>
public class PedidoRequestDto
{
    [Required]
    public Guid ClienteId { get; set; }

    [MaxLength(500)]
    public string? Observacoes { get; set; }

    [Required, MinLength(1)]
    public List<ItemPedidoRequestDto> Itens { get; set; } = new();
}

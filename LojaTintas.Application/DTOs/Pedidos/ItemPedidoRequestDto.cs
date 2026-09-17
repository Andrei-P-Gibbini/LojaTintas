using System.ComponentModel.DataAnnotations;

namespace LojaTintas.Application.DTOs.Pedidos;

/// <summary>Item informado ao criar um pedido.</summary>
public class ItemPedidoRequestDto
{
    [Required]
    public Guid ProdutoId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantidade { get; set; }

    [Range(0, 100)]
    public decimal DescontoPercentual { get; set; } = 0;
}

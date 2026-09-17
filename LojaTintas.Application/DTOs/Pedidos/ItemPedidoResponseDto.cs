namespace LojaTintas.Application.DTOs.Pedidos;

/// <summary>Representação de um item de pedido retornado pela API.</summary>
public class ItemPedidoResponseDto
{
    public int Id { get; set; }
    public Guid ProdutoId { get; set; }
    public string? ProdutoNome { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal DescontoPercentual { get; set; }
    public decimal Subtotal => Math.Round(Quantidade * PrecoUnitario * (1 - DescontoPercentual / 100), 2);
}

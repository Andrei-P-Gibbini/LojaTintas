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

    /// <summary>
    /// Adiciona um item ao pedido, tirando o snapshot do preço de venda do produto
    /// no momento da venda. Invariantes de negócio: quantidade deve ser positiva e
    /// desconto deve estar entre 0 e 100%. Recalcula o valor total do pedido.
    /// </summary>
    public ItemPedido AdicionarItem(Produto produto, int quantidade, decimal descontoPercentual)
    {
        ArgumentNullException.ThrowIfNull(produto);

        if (quantidade <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantidade), quantidade, "A quantidade deve ser maior que zero.");

        if (descontoPercentual < 0 || descontoPercentual > 100)
            throw new ArgumentOutOfRangeException(nameof(descontoPercentual), descontoPercentual, "O desconto deve estar entre 0 e 100.");

        var item = new ItemPedido
        {
            ProdutoId = produto.Id,
            Produto = produto,
            Quantidade = quantidade,
            PrecoUnitario = produto.PrecoVenda,
            DescontoPercentual = descontoPercentual
        };

        Itens.Add(item);
        RecalcularValorTotal();

        return item;
    }

    /// <summary>Recalcula o valor total do pedido a partir dos itens atuais.</summary>
    public void RecalcularValorTotal()
    {
        ValorTotal = Itens.Sum(i => Math.Round(i.Quantidade * i.PrecoUnitario * (1 - i.DescontoPercentual / 100), 2));
    }
}

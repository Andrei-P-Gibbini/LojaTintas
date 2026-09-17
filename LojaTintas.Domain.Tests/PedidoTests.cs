using LojaTintas.Domain.Entities;
using LojaTintas.Domain.Enums;
using Xunit;

namespace LojaTintas.Domain.Tests;

/// <summary>
/// Testes de domínio (CP4) — sem mock, sem Infrastructure. Cobrem a regra de negócio
/// real de Pedido.AdicionarItem: invariantes de quantidade e desconto, e o
/// recálculo do valor total.
/// </summary>
public class PedidoTests
{
    private static Produto CriarProdutoValido(decimal precoVenda = 89.90m) => new()
    {
        Nome = "Tinta Acrílica Branca",
        CodigoSku = "TIN-001",
        PrecoVenda = precoVenda,
        VolumeLitros = 3.6m,
        TipoTinta = TipoTinta.Acrilica,
        CategoriaId = 1,
        FabricanteId = 1
    };

    [Fact]
    public void AdicionarItem_ComDadosValidos_DeveAdicionarItemERecalcularValorTotal()
    {
        // Arrange
        var pedido = new Pedido { NumeroPedido = "PED-TESTE-001", ClienteId = Guid.NewGuid() };
        var produto = CriarProdutoValido(precoVenda: 100m);

        // Act
        var item = pedido.AdicionarItem(produto, quantidade: 2, descontoPercentual: 10);

        // Assert
        Assert.Single(pedido.Itens);
        Assert.Same(item, pedido.Itens.First());
        Assert.Equal(100m, item.PrecoUnitario);
        // 2 unidades * R$100 com 10% de desconto = 180
        Assert.Equal(180m, pedido.ValorTotal);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(150)]
    public void AdicionarItem_ComDescontoForaDoIntervalo_DeveLancarArgumentOutOfRangeException(decimal descontoInvalido)
    {
        // Arrange
        var pedido = new Pedido { NumeroPedido = "PED-TESTE-002", ClienteId = Guid.NewGuid() };
        var produto = CriarProdutoValido();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            pedido.AdicionarItem(produto, quantidade: 1, descontoPercentual: descontoInvalido));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void AdicionarItem_ComQuantidadeInvalida_DeveLancarArgumentOutOfRangeException(int quantidadeInvalida)
    {
        // Arrange
        var pedido = new Pedido { NumeroPedido = "PED-TESTE-003", ClienteId = Guid.NewGuid() };
        var produto = CriarProdutoValido();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            pedido.AdicionarItem(produto, quantidade: quantidadeInvalida, descontoPercentual: 0));
    }

    [Fact]
    public void AdicionarItem_ComProdutoNulo_DeveLancarArgumentNullException()
    {
        // Arrange
        var pedido = new Pedido { NumeroPedido = "PED-TESTE-004", ClienteId = Guid.NewGuid() };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            pedido.AdicionarItem(null!, quantidade: 1, descontoPercentual: 0));
    }
}

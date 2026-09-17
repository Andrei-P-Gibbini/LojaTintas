using LojaTintas.Application.DTOs.Pedidos;
using LojaTintas.Application.Interfaces.Repositories;
using LojaTintas.Application.Services;
using LojaTintas.Domain.Entities;
using LojaTintas.Domain.Enums;
using LojaTintas.Domain.Exceptions;
using Moq;
using Xunit;

namespace LojaTintas.Application.Tests.Services;

/// <summary>
/// Testes de aplicação (CP4) — repositórios mockados com Moq. Cobrem o cenário de
/// dependência ausente (não deve persistir) e o caminho feliz (persiste uma vez).
/// </summary>
public class PedidoServiceTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepositoryMock = new();
    private readonly Mock<IClienteRepository> _clienteRepositoryMock = new();
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock = new();
    private readonly PedidoService _service;

    public PedidoServiceTests()
    {
        _service = new PedidoService(
            _pedidoRepositoryMock.Object,
            _clienteRepositoryMock.Object,
            _produtoRepositoryMock.Object);
    }

    private static Cliente CriarClienteValido() => new()
    {
        Id = Guid.NewGuid(),
        Nome = "Cliente Teste",
        CpfCnpj = "12345678900",
        Email = "cliente@teste.com",
        Telefone = "11999999999"
    };

    private static Produto CriarProdutoValido(decimal precoVenda = 50m) => new()
    {
        Id = Guid.NewGuid(),
        Nome = "Tinta Teste",
        CodigoSku = "SKU-TESTE",
        PrecoVenda = precoVenda,
        VolumeLitros = 1m,
        TipoTinta = TipoTinta.Latex,
        CategoriaId = 1,
        FabricanteId = 1
    };

    [Fact]
    public async Task CriarPedidoAsync_ComClienteInexistente_DeveLancarResourceNotFoundExceptionENaoPersistir()
    {
        // Arrange
        _clienteRepositoryMock
            .Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Cliente?)null);

        var dto = new PedidoRequestDto
        {
            ClienteId = Guid.NewGuid(),
            Itens = [new ItemPedidoRequestDto { ProdutoId = Guid.NewGuid(), Quantidade = 1 }]
        };

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.CriarPedidoAsync(dto));

        _pedidoRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Pedido>()), Times.Never);
    }

    [Fact]
    public async Task CriarPedidoAsync_ComProdutoInexistente_DeveLancarResourceNotFoundExceptionENaoPersistir()
    {
        // Arrange
        var cliente = CriarClienteValido();
        _clienteRepositoryMock.Setup(r => r.ObterPorIdAsync(cliente.Id)).ReturnsAsync(cliente);
        _produtoRepositoryMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Produto?)null);

        var dto = new PedidoRequestDto
        {
            ClienteId = cliente.Id,
            Itens = [new ItemPedidoRequestDto { ProdutoId = Guid.NewGuid(), Quantidade = 1 }]
        };

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.CriarPedidoAsync(dto));

        _pedidoRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Pedido>()), Times.Never);
    }

    [Fact]
    public async Task CriarPedidoAsync_ComClienteEProdutoValidos_DevePersistirUmaVez()
    {
        // Arrange
        var cliente = CriarClienteValido();
        var produto = CriarProdutoValido(precoVenda: 100m);
        Pedido? pedidoCriado = null;

        _clienteRepositoryMock.Setup(r => r.ObterPorIdAsync(cliente.Id)).ReturnsAsync(cliente);
        _produtoRepositoryMock.Setup(r => r.ObterPorIdAsync(produto.Id)).ReturnsAsync(produto);

        _pedidoRepositoryMock
            .Setup(r => r.AdicionarAsync(It.IsAny<Pedido>()))
            .Callback<Pedido>(p => pedidoCriado = p)
            .Returns(Task.CompletedTask);

        _pedidoRepositoryMock
            .Setup(r => r.ObterComItensAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => pedidoCriado);

        var dto = new PedidoRequestDto
        {
            ClienteId = cliente.Id,
            Itens = [new ItemPedidoRequestDto { ProdutoId = produto.Id, Quantidade = 2, DescontoPercentual = 0 }]
        };

        // Act
        var resultado = await _service.CriarPedidoAsync(dto);

        // Assert
        Assert.Equal(200m, resultado.ValorTotal);
        Assert.Equal(cliente.Id, resultado.ClienteId);
        _pedidoRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Pedido>()), Times.Once);
    }
}

using LojaTintas.Application.DTOs.Common;
using LojaTintas.Application.Interfaces.Repositories;
using LojaTintas.Application.Services;
using LojaTintas.Domain.Entities;
using Moq;
using Xunit;

namespace LojaTintas.Application.Tests.Services;

public class PedidoServicePaginacaoTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepositoryMock = new();
    private readonly Mock<IClienteRepository> _clienteRepositoryMock = new();
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock = new();
    private readonly PedidoService _service;

    public PedidoServicePaginacaoTests()
    {
        _service = new PedidoService(
            _pedidoRepositoryMock.Object,
            _clienteRepositoryMock.Object,
            _produtoRepositoryMock.Object);
    }

    private static Pedido CriarPedido(string numero) => new()
    {
        NumeroPedido = numero,
        ClienteId = Guid.NewGuid()
    };

    [Theory]
    [InlineData(0, 20)]
    [InlineData(-3, 20)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    [InlineData(1, 9999)]
    public async Task ObterPaginadoAsync_ComPageOuPageSizeInvalido_DeveLancarArgumentExceptionENaoConsultarORepositorio(
        int page, int pageSize)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.ObterPaginadoAsync(page, pageSize));

        _pedidoRepositoryMock.Verify(r => r.GetPagedAsync(It.IsAny<PaginationParams>()), Times.Never);
    }

    [Fact]
    public async Task ObterPaginadoAsync_ComIntervaloValido_DeveMontarEnvelopeComTotais()
    {
        // Arrange
        IReadOnlyList<Pedido> paginaDoBanco = [CriarPedido("PED-3"), CriarPedido("PED-4")];
        _pedidoRepositoryMock
            .Setup(r => r.GetPagedAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync((paginaDoBanco, 5));

        // Act
        var resultado = await _service.ObterPaginadoAsync(page: 2, pageSize: 2);

        // Assert
        Assert.Equal(2, resultado.Page);
        Assert.Equal(2, resultado.PageSize);
        Assert.Equal(5, resultado.TotalItems);
        Assert.Equal(3, resultado.TotalPages);
        Assert.True(resultado.HasPrevious);
        Assert.True(resultado.HasNext);
        var numeros = resultado.Items.Select(i => i.NumeroPedido).ToArray();
        Assert.Equal(new[] { "PED-3", "PED-4" }, numeros);

        _pedidoRepositoryMock.Verify(
            r => r.GetPagedAsync(It.Is<PaginationParams>(p => p.Page == 2 && p.PageSize == 2)),
            Times.Once);
    }

    [Fact]
    public async Task ObterPaginadoAsync_ComPaginaAlemDoTotal_DeveDevolverItemsVazioSemErro()
    {
        // Arrange
        _pedidoRepositoryMock
            .Setup(r => r.GetPagedAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync((Array.Empty<Pedido>(), 3));

        // Act
        var resultado = await _service.ObterPaginadoAsync(page: 999, pageSize: 20);

        // Assert
        Assert.Empty(resultado.Items);
        Assert.Equal(3, resultado.TotalItems);
        Assert.Equal(1, resultado.TotalPages);
        Assert.False(resultado.HasNext);
    }
}

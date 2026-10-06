using LojaTintas.Application.DTOs.Common;
using LojaTintas.Application.DTOs.Pedidos;
using LojaTintas.Application.Interfaces.Repositories;
using LojaTintas.Domain.Entities;
using LojaTintas.Domain.Exceptions;

namespace LojaTintas.Application.Services;

/// <summary>
/// Orquestra a criação de pedidos: valida cliente e produtos, tira o snapshot
/// de preço em cada item e calcula o valor total. Mantém o PedidosController
/// enxuto, sem regra de negócio na camada HTTP.
/// </summary>
public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IProdutoRepository _produtoRepository;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IClienteRepository clienteRepository,
        IProdutoRepository produtoRepository)
    {
        _pedidoRepository = pedidoRepository;
        _clienteRepository = clienteRepository;
        _produtoRepository = produtoRepository;
    }

    public async Task<PedidoResponseDto> CriarPedidoAsync(PedidoRequestDto dto)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(dto.ClienteId)
            ?? throw new ResourceNotFoundException(nameof(Cliente), dto.ClienteId);

        var pedido = new Pedido
        {
            NumeroPedido = GerarNumeroPedido(),
            ClienteId = cliente.Id,
            Observacoes = dto.Observacoes
        };

        foreach (var itemDto in dto.Itens)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(itemDto.ProdutoId)
                ?? throw new ResourceNotFoundException(nameof(Produto), itemDto.ProdutoId);

            // Regra de negócio (invariantes de quantidade/desconto e cálculo do total) vive no Domain.
            pedido.AdicionarItem(produto, itemDto.Quantidade, itemDto.DescontoPercentual);
        }

        await _pedidoRepository.AdicionarAsync(pedido);

        var criado = await _pedidoRepository.ObterComItensAsync(pedido.Id);
        return MapToDto(criado!);
    }

    public async Task<PedidoResponseDto?> ObterComItensAsync(Guid id)
    {
        var pedido = await _pedidoRepository.ObterComItensAsync(id);
        return pedido is null ? null : MapToDto(pedido);
    }

    public async Task<IEnumerable<PedidoResponseDto>> ObterTodosAsync()
    {
        // Listagem "enxuta" (sem Include) — para itens e nome do cliente, usar ObterComItensAsync.
        var pedidos = await _pedidoRepository.ObterTodosAsync();
        return pedidos.Select(MapToDto);
    }

    public async Task<PagedResult<PedidoResponseDto>> ObterPaginadoAsync(int page, int pageSize)
    {
        var pagination = PaginationParams.Create(page, pageSize);

        var (pedidos, totalItems) = await _pedidoRepository.GetPagedAsync(pagination);

        var items = pedidos.Select(MapToDto).ToList();
        return PagedResult<PedidoResponseDto>.Create(items, pagination.Page, pagination.PageSize, totalItems);
    }

    private static string GerarNumeroPedido()
        => $"PED-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";

    private static PedidoResponseDto MapToDto(Pedido pedido) => new()
    {
        Id = pedido.Id,
        NumeroPedido = pedido.NumeroPedido,
        DataPedido = pedido.DataPedido,
        Status = pedido.Status,
        ValorTotal = pedido.ValorTotal,
        Observacoes = pedido.Observacoes,
        ClienteId = pedido.ClienteId,
        ClienteNome = pedido.Cliente?.Nome,
        Itens = pedido.Itens.Select(i => new ItemPedidoResponseDto
        {
            Id = i.Id,
            ProdutoId = i.ProdutoId,
            ProdutoNome = i.Produto?.Nome,
            Quantidade = i.Quantidade,
            PrecoUnitario = i.PrecoUnitario,
            DescontoPercentual = i.DescontoPercentual
        }).ToList()
    };
}

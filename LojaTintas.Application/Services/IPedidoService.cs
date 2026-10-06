using LojaTintas.Application.DTOs.Common;
using LojaTintas.Application.DTOs.Pedidos;

namespace LojaTintas.Application.Services;

public interface IPedidoService
{
    Task<PedidoResponseDto> CriarPedidoAsync(PedidoRequestDto dto);
    Task<PedidoResponseDto?> ObterComItensAsync(Guid id);
    Task<IEnumerable<PedidoResponseDto>> ObterTodosAsync();

    Task<PagedResult<PedidoResponseDto>> ObterPaginadoAsync(int page, int pageSize);
}

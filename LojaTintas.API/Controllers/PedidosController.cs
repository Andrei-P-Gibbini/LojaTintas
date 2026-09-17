using LojaTintas.Application.DTOs.Pedidos;
using LojaTintas.Application.Services;
using LojaTintas.Domain.Entities;
using LojaTintas.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace LojaTintas.API.Controllers;

/// <summary>Criação e consulta de pedidos de venda.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;
    private readonly ILogger<PedidosController> _logger;

    public PedidosController(IPedidoService pedidoService, ILogger<PedidosController> logger)
    {
        _pedidoService = pedidoService;
        _logger = logger;
    }

    /// <summary>Lista todos os pedidos (sem itens — use a busca por id para o detalhe).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PedidoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTodos()
        => Ok(await _pedidoService.ObterTodosAsync());

    /// <summary>Busca um pedido com seus itens.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PedidoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var pedido = await _pedidoService.ObterComItensAsync(id)
            ?? throw new ResourceNotFoundException(nameof(Pedido), id);

        return Ok(pedido);
    }

    /// <summary>Cria um novo pedido a partir de um cliente e uma lista de itens.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(PedidoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Criar([FromBody] PedidoRequestDto dto)
    {
        _logger.LogInformation(
            "Iniciando criação de pedido. ClienteId={ClienteId} QuantidadeItens={QuantidadeItens} TraceId={TraceId}",
            dto.ClienteId, dto.Itens.Count, HttpContext.TraceIdentifier);

        var pedido = await _pedidoService.CriarPedidoAsync(dto);

        _logger.LogInformation(
            "Pedido criado com sucesso. PedidoId={PedidoId} NumeroPedido={NumeroPedido} ValorTotal={ValorTotal} TraceId={TraceId}",
            pedido.Id, pedido.NumeroPedido, pedido.ValorTotal, HttpContext.TraceIdentifier);

        return CreatedAtAction(nameof(ObterPorId), new { id = pedido.Id }, pedido);
    }
}

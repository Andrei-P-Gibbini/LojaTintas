using Asp.Versioning;
using LojaTintas.Application.DTOs.Common;
using LojaTintas.Application.DTOs.Pedidos;
using LojaTintas.Application.Services;
using LojaTintas.Domain.Entities;
using LojaTintas.Domain.Exceptions;
using LojaTintas.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LojaTintas.API.Controllers;

/// <summary>
/// Criação e consulta de pedidos de venda. Único recurso versionado (CP5):
/// a 1.0 (deprecada) devolve a lista antiga; a 2.0 (atual, padrão) devolve a lista paginada.
/// As duas versões usam o mesmo <see cref="IPedidoService"/>.
/// </summary>
[ApiController]
[ApiVersion("1.0", Deprecated = true)]
[ApiVersion("2.0")]
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

    /// <summary>[DEPRECADA — use a v2] Lista todos os pedidos, sem paginação (contrato antigo do CP3).</summary>
    /// <remarks>
    /// Devolve o array completo, sem itens (use a busca por id para o detalhe). Esta versão segue
    /// no ar para quem ainda a consome, mas será desligada: migre para a 2.0, que pagina.
    /// </remarks>
    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(IEnumerable<PedidoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTodos()
        => Ok(await _pedidoService.ObterTodosAsync());

    /// <summary>Lista pedidos paginados (v2, versão atual e padrão).</summary>
    /// <remarks>
    /// Corpo: <c>page</c>, <c>pageSize</c>, <c>totalItems</c>, <c>totalPages</c>, <c>hasPrevious</c>,
    /// <c>hasNext</c> e <c>items</c>. Ordenado do pedido mais recente para o mais antigo.
    /// Página além do total devolve 200 com <c>items</c> vazio.
    /// </remarks>
    /// <param name="page">Número da página, a partir de 1 (padrão 1).</param>
    /// <param name="pageSize">Itens por página, de 1 a 100 (padrão 20).</param>
    [HttpGet]
    [MapToApiVersion("2.0")]
    [EnableRateLimiting(RateLimitingServiceExtensions.LeituraPolicy)]
    [ProducesResponseType(typeof(PagedResult<PedidoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> ObterPaginado(
        [FromQuery] int page = PaginationParams.DefaultPage,
        [FromQuery] int pageSize = PaginationParams.DefaultPageSize)
        => Ok(await _pedidoService.ObterPaginadoAsync(page, pageSize));

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
    /// <remarks>
    /// Limitado por rate limit (política <c>escrita</c>): 10 requisições por minuto por IP.
    /// Ao estourar, responde 429 com o header <c>Retry-After</c>.
    /// </remarks>
    [HttpPost]
    [EnableRateLimiting(RateLimitingServiceExtensions.EscritaPolicy)]
    [ProducesResponseType(typeof(PedidoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
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

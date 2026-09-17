using LojaTintas.Application.DTOs.Produtos;
using LojaTintas.Application.Interfaces.Repositories;
using LojaTintas.Domain.Entities;
using LojaTintas.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace LojaTintas.API.Controllers;

/// <summary>Consulta e cadastro de produtos.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IRepository<Categoria> _categoriaRepository;
    private readonly IRepository<Fabricante> _fabricanteRepository;

    public ProdutosController(
        IProdutoRepository produtoRepository,
        IRepository<Categoria> categoriaRepository,
        IRepository<Fabricante> fabricanteRepository)
    {
        _produtoRepository = produtoRepository;
        _categoriaRepository = categoriaRepository;
        _fabricanteRepository = fabricanteRepository;
    }

    /// <summary>Lista todos os produtos cadastrados.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProdutoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTodos()
    {
        var produtos = await _produtoRepository.ObterTodosAsync();
        return Ok(produtos.Select(ToDto));
    }

    /// <summary>Busca um produto pelo id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id)
            ?? throw new ResourceNotFoundException(nameof(Produto), id);

        return Ok(ToDto(produto));
    }

    /// <summary>Lista os produtos de uma categoria.</summary>
    [HttpGet("por-categoria/{categoriaId:int}")]
    [ProducesResponseType(typeof(IEnumerable<ProdutoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterPorCategoria(int categoriaId)
    {
        var produtos = await _produtoRepository.ObterPorCategoriaAsync(categoriaId);
        return Ok(produtos.Select(ToDto));
    }

    /// <summary>Cadastra um novo produto.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Criar([FromBody] ProdutoRequestDto dto)
    {
        if (await _produtoRepository.ObterPorSkuAsync(dto.CodigoSku) is not null)
            throw new ConflictException($"Já existe um produto com o SKU '{dto.CodigoSku}'.");

        if (await _categoriaRepository.GetByIdAsync(dto.CategoriaId) is null)
            throw new ResourceNotFoundException(nameof(Categoria), dto.CategoriaId);

        if (await _fabricanteRepository.GetByIdAsync(dto.FabricanteId) is null)
            throw new ResourceNotFoundException(nameof(Fabricante), dto.FabricanteId);

        var produto = new Produto
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            CodigoSku = dto.CodigoSku,
            PrecoVenda = dto.PrecoVenda,
            VolumeLitros = dto.VolumeLitros,
            TipoTinta = dto.TipoTinta,
            CorBase = dto.CorBase,
            CategoriaId = dto.CategoriaId,
            FabricanteId = dto.FabricanteId
        };

        await _produtoRepository.AdicionarAsync(produto);

        var criado = await _produtoRepository.ObterPorSkuAsync(produto.CodigoSku);
        return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, ToDto(criado!));
    }

    private static ProdutoResponseDto ToDto(Produto p) => new()
    {
        Id = p.Id,
        Nome = p.Nome,
        Descricao = p.Descricao,
        CodigoSku = p.CodigoSku,
        PrecoVenda = p.PrecoVenda,
        VolumeLitros = p.VolumeLitros,
        TipoTinta = p.TipoTinta,
        CorBase = p.CorBase,
        CategoriaId = p.CategoriaId,
        CategoriaNome = p.Categoria?.Nome,
        FabricanteId = p.FabricanteId,
        FabricanteNome = p.Fabricante?.Nome
    };
}

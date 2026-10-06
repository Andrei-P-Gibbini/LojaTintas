using Asp.Versioning;
using LojaTintas.Application.DTOs.Categorias;
using LojaTintas.Application.Interfaces.Repositories;
using LojaTintas.Domain.Entities;
using LojaTintas.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace LojaTintas.API.Controllers;

/// <summary>
/// CRUD de categorias de produtos. Demonstra o uso direto do repositório
/// genérico (<c>IRepository&lt;T&gt;</c>) exigido pelo CP3.
/// </summary>
[ApiController]
[ApiVersionNeutral]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriasController : ControllerBase
{
    private readonly IRepository<Categoria> _repository;

    public CategoriasController(IRepository<Categoria> repository)
    {
        _repository = repository;
    }

    /// <summary>Lista todas as categorias cadastradas.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoriaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTodas()
    {
        var categorias = await _repository.GetAllAsync();
        return Ok(categorias.Select(ToDto));
    }

    /// <summary>Busca uma categoria pelo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoriaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var categoria = await _repository.GetByIdAsync(id)
            ?? throw new ResourceNotFoundException(nameof(Categoria), id);

        return Ok(ToDto(categoria));
    }

    /// <summary>Cria uma nova categoria.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CategoriaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CategoriaRequestDto dto)
    {
        var categoria = new Categoria { Nome = dto.Nome, Descricao = dto.Descricao };
        await _repository.AddAsync(categoria);

        return CreatedAtAction(nameof(ObterPorId), new { id = categoria.Id }, ToDto(categoria));
    }

    /// <summary>Atualiza uma categoria existente.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] CategoriaRequestDto dto)
    {
        var categoria = await _repository.GetByIdAsync(id)
            ?? throw new ResourceNotFoundException(nameof(Categoria), id);

        categoria.Nome = dto.Nome;
        categoria.Descricao = dto.Descricao;

        await _repository.UpdateAsync(categoria);
        return NoContent();
    }

    /// <summary>Remove uma categoria.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id)
    {
        if (!await _repository.ExistsByIdAsync(id))
            throw new ResourceNotFoundException(nameof(Categoria), id);

        await _repository.DeleteAsync(id);
        return NoContent();
    }

    private static CategoriaResponseDto ToDto(Categoria c) => new()
    {
        Id = c.Id,
        Nome = c.Nome,
        Descricao = c.Descricao
    };
}

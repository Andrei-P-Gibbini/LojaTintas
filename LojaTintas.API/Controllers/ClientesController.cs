using LojaTintas.Application.DTOs.Clientes;
using LojaTintas.Application.Interfaces.Repositories;
using LojaTintas.Domain.Entities;
using LojaTintas.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace LojaTintas.API.Controllers;

/// <summary>Cadastro e consulta de clientes.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ClientesController : ControllerBase
{
    private readonly IClienteRepository _clienteRepository;

    public ClientesController(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    /// <summary>Lista todos os clientes cadastrados.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClienteResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTodos()
    {
        var clientes = await _clienteRepository.ObterTodosAsync();
        return Ok(clientes.Select(ToDto));
    }

    /// <summary>Busca um cliente pelo id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(id)
            ?? throw new ResourceNotFoundException(nameof(Cliente), id);

        return Ok(ToDto(cliente));
    }

    /// <summary>Cadastra um novo cliente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClienteResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Criar([FromBody] ClienteRequestDto dto)
    {
        if (await _clienteRepository.ObterPorCpfCnpjAsync(dto.CpfCnpj) is not null)
            throw new ConflictException($"Já existe um cliente com o CPF/CNPJ '{dto.CpfCnpj}'.");

        if (await _clienteRepository.ObterPorEmailAsync(dto.Email) is not null)
            throw new ConflictException($"Já existe um cliente com o e-mail '{dto.Email}'.");

        var cliente = new Cliente
        {
            Nome = dto.Nome,
            CpfCnpj = dto.CpfCnpj,
            Email = dto.Email,
            Telefone = dto.Telefone,
            Endereco = dto.Endereco
        };

        await _clienteRepository.AdicionarAsync(cliente);

        return CreatedAtAction(nameof(ObterPorId), new { id = cliente.Id }, ToDto(cliente));
    }

    private static ClienteResponseDto ToDto(Cliente c) => new()
    {
        Id = c.Id,
        Nome = c.Nome,
        CpfCnpj = c.CpfCnpj,
        Email = c.Email,
        Telefone = c.Telefone,
        Endereco = c.Endereco,
        DataCadastro = c.DataCadastro
    };
}

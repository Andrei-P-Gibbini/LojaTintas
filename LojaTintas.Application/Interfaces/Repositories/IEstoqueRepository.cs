using LojaTintas.Domain.Entities;

namespace LojaTintas.Application.Interfaces.Repositories;

public interface IEstoqueRepository : IRepository<Estoque, int>
{
    Task<Estoque?> ObterPorProdutoAsync(Guid produtoId);
    Task<IEnumerable<Estoque>> ObterAbaixoDoMinimoAsync();
}

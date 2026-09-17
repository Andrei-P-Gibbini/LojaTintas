using LojaTintas.Domain.Entities;

namespace LojaTintas.Application.Interfaces.Repositories;

public interface IProdutoRepository : IRepository<Produto, Guid>
{
    Task<Produto?> ObterPorSkuAsync(string sku);
    Task<IEnumerable<Produto>> ObterPorCategoriaAsync(int categoriaId);
    Task<IEnumerable<Produto>> ObterComEstoqueBaixoAsync();
}

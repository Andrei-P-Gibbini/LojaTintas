using LojaTintas.Application.Interfaces.Repositories;
using LojaTintas.Domain.Entities;
using LojaTintas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LojaTintas.Infrastructure.Repositories;

public class EstoqueRepository : Repository<Estoque, int>, IEstoqueRepository
{
    public EstoqueRepository(LojaTintasDbContext context) : base(context) { }

    public async Task<Estoque?> ObterPorProdutoAsync(Guid produtoId)
        => await _dbSet
            .AsNoTracking()
            .Include(e => e.Produto)
            .FirstOrDefaultAsync(e => e.ProdutoId == produtoId);

    public async Task<IEnumerable<Estoque>> ObterAbaixoDoMinimoAsync()
        => await _dbSet
            .AsNoTracking()
            .Include(e => e.Produto)
            .Where(e => e.QuantidadeAtual < e.QuantidadeMinima)
            .ToListAsync();
}

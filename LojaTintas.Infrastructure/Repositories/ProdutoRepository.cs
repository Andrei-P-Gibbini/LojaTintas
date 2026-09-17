using LojaTintas.Application.Interfaces.Repositories;
using LojaTintas.Domain.Entities;
using LojaTintas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LojaTintas.Infrastructure.Repositories;

public class ProdutoRepository : Repository<Produto, Guid>, IProdutoRepository
{
    public ProdutoRepository(LojaTintasDbContext context) : base(context) { }

    public async Task<Produto?> ObterPorSkuAsync(string sku)
        => await _dbSet
            .AsNoTracking()
            .Include(p => p.Categoria)
            .Include(p => p.Fabricante)
            .FirstOrDefaultAsync(p => p.CodigoSku == sku);

    public async Task<IEnumerable<Produto>> ObterPorCategoriaAsync(int categoriaId)
        => await _dbSet
            .AsNoTracking()
            .Where(p => p.CategoriaId == categoriaId)
            .Include(p => p.Fabricante)
            .ToListAsync();

    public async Task<IEnumerable<Produto>> ObterComEstoqueBaixoAsync()
        => await _dbSet
            .AsNoTracking()
            .Include(p => p.Estoque)
            .Where(p => p.Estoque != null && p.Estoque.QuantidadeAtual < p.Estoque.QuantidadeMinima)
            .ToListAsync();
}

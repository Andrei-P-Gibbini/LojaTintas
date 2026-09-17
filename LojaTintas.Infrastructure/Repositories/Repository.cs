using LojaTintas.Application.Interfaces.Repositories;
using LojaTintas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LojaTintas.Infrastructure.Repositories;

public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class
{
    protected readonly LojaTintasDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(LojaTintasDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> ObterPorIdAsync(TKey id)
        => await _dbSet.FindAsync(id);

    public virtual async Task<IEnumerable<TEntity>> ObterTodosAsync()
        => await _dbSet.AsNoTracking().ToListAsync();

    public virtual async Task AdicionarAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task AtualizarAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task RemoverAsync(TKey id)
    {
        var entity = await ObterPorIdAsync(id);
        if (entity is null) return;

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}

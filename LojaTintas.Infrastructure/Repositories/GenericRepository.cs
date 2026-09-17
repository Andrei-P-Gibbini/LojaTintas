using LojaTintas.Application.Interfaces.Repositories;
using LojaTintas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LojaTintas.Infrastructure.Repositories;

/// <summary>
/// Implementação EF Core do repositório genérico (CP3). Distinta de
/// <see cref="Repository{TEntity, TKey}"/> (classe base dos repositórios específicos
/// por agregado do CP2) — aqui a aridade genérica é 1, resolvendo por
/// DbSet&lt;T&gt;.FindAsync, que aceita qualquer formato de chave primária via
/// params object[].
/// </summary>
public class Repository<T> : IRepository<T> where T : class
{
    private readonly LojaTintasDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(LojaTintasDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
        => await _dbSet.AsNoTracking().ToListAsync();

    public async Task<T?> GetByIdAsync(params object[] keyValues)
        => await _dbSet.FindAsync(keyValues);

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
            _dbSet.Update(entity);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(params object[] keyValues)
    {
        var entity = await _dbSet.FindAsync(keyValues);
        if (entity is null) return;

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByIdAsync(params object[] keyValues)
        => await _dbSet.FindAsync(keyValues) is not null;
}

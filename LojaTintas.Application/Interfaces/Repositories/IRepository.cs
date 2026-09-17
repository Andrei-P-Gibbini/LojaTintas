namespace LojaTintas.Application.Interfaces.Repositories;

public interface IRepository<TEntity, TKey> where TEntity : class
{
    Task<TEntity?> ObterPorIdAsync(TKey id);
    Task<IEnumerable<TEntity>> ObterTodosAsync();
    Task AdicionarAsync(TEntity entity);
    Task AtualizarAsync(TEntity entity);
    Task RemoverAsync(TKey id);
}

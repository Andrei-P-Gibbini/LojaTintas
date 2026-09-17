namespace LojaTintas.Application.Interfaces.Repositories;

/// <summary>
/// Repositório genérico exigido pelo CP3 — contrato único de CRUD básico para
/// qualquer entidade, independentemente do tipo da chave primária (int, Guid ou
/// chave composta). Convive com os repositórios específicos por agregado
/// (IProdutoRepository, IClienteRepository, etc.) já usados desde o CP2, que
/// continuam existindo para consultas além do CRUD mínimo (ex.: busca por SKU).
/// Registrado como serviço aberto na DI:
/// services.AddScoped(typeof(IRepository&lt;&gt;), typeof(Repository&lt;&gt;)).
/// </summary>
/// <typeparam name="T">Tipo da entidade.</typeparam>
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>Busca por chave primária. Aceita chaves compostas (ex.: ProdutoFornecedor).</summary>
    Task<T?> GetByIdAsync(params object[] keyValues);

    Task AddAsync(T entity);

    /// <summary>Persiste alterações em uma entidade já obtida via GetByIdAsync.</summary>
    Task UpdateAsync(T entity);

    Task DeleteAsync(params object[] keyValues);

    Task<bool> ExistsByIdAsync(params object[] keyValues);
}

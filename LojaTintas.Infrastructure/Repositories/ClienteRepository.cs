using LojaTintas.Application.Interfaces.Repositories;
using LojaTintas.Domain.Entities;
using LojaTintas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LojaTintas.Infrastructure.Repositories;

public class ClienteRepository : Repository<Cliente, Guid>, IClienteRepository
{
    public ClienteRepository(LojaTintasDbContext context) : base(context) { }

    public async Task<Cliente?> ObterPorCpfCnpjAsync(string cpfCnpj)
        => await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.CpfCnpj == cpfCnpj);

    public async Task<Cliente?> ObterPorEmailAsync(string email)
        => await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Email == email);

    public async Task<IEnumerable<Cliente>> ObterComPedidosAsync()
        => await _dbSet.AsNoTracking().Include(c => c.Pedidos).ToListAsync();
}

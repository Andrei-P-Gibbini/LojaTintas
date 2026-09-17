using LojaTintas.Domain.Entities;
using LojaTintas.Domain.Enums;
using LojaTintas.Infrastructure.Data;
using LojaTintas.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LojaTintas.Tests;

public class ProdutoRepositoryTests
{
    private LojaTintasDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<LojaTintasDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new LojaTintasDbContext(options);
    }

    [Fact]
    public async Task AdicionarProduto_DeveSalvarERecuperarPeloSku()
    {
        // Arrange — preparar os dados
        using var ctx = CriarContexto();
        var repo = new ProdutoRepository(ctx);

        var categoria = new Categoria { Nome = "Tintas" };
        var fabricante = new Fabricante { Nome = "Suvinil", Cnpj = "00.000.000/0001-00" };
        ctx.Categorias.Add(categoria);
        ctx.Fabricantes.Add(fabricante);
        await ctx.SaveChangesAsync();

        var produto = new Produto
        {
            Nome = "Tinta Branca",
            CodigoSku = "SKU-001",
            PrecoVenda = 49.90m,
            VolumeLitros = 3.6m,
            TipoTinta = TipoTinta.Latex,
            CategoriaId = categoria.Id,
            FabricanteId = fabricante.Id
        };

        // Act — executar o que está sendo testado
        await repo.AdicionarAsync(produto);
        var resultado = await repo.ObterPorSkuAsync("SKU-001");

        // Assert — verificar se deu certo
        Assert.NotNull(resultado);
        Assert.Equal("Tinta Branca", resultado.Nome);
    }
}
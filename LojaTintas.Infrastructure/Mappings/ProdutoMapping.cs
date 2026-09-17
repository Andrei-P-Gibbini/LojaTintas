using LojaTintas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaTintas.Infrastructure.Mappings;

public class ProdutoMapping : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Descricao)
            .HasMaxLength(1000);

        builder.Property(p => p.CodigoSku)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.PrecoVenda)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(p => p.VolumeLitros)
            .IsRequired()
            .HasColumnType("decimal(6,2)");

        builder.Property(p => p.TipoTinta)
            .IsRequired();

        builder.Property(p => p.CorBase)
            .HasMaxLength(80);

        builder.HasIndex(p => p.CodigoSku).IsUnique();

        // N:1 → Categoria (obrigatório)
        builder.HasOne(p => p.Categoria)
            .WithMany(c => c.Produtos)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        // N:1 → Fabricante (obrigatório)
        builder.HasOne(p => p.Fabricante)
            .WithMany(f => f.Produtos)
            .HasForeignKey(p => p.FabricanteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

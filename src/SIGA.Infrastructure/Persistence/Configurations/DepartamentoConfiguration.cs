using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence.Configurations;

public class DepartamentoConfiguration : IEntityTypeConfiguration<Departamento>
{
    public void Configure(EntityTypeBuilder<Departamento> builder)
    {
        builder.ToTable("Departamentos");

        builder.Property(d => d.Nombre).IsRequired().HasMaxLength(100);
        builder.HasIndex(d => d.Nombre).IsUnique();

        builder.HasMany(d => d.Provincias)
            .WithOne(p => p.Departamento)
            .HasForeignKey(p => p.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

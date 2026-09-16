using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence.Configurations;

public class ProvinciaConfiguration : IEntityTypeConfiguration<Provincia>
{
    public void Configure(EntityTypeBuilder<Provincia> builder)
    {
        builder.ToTable("Provincias");

        builder.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
        builder.HasIndex(p => new { p.DepartamentoId, p.Nombre }).IsUnique();

        builder.HasMany(p => p.Municipios)
            .WithOne(m => m.Provincia)
            .HasForeignKey(m => m.ProvinciaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

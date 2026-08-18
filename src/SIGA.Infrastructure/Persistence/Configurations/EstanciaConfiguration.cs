using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence.Configurations;

public class EstanciaConfiguration : IEntityTypeConfiguration<Estancia>
{
    public void Configure(EntityTypeBuilder<Estancia> builder)
    {
        builder.ToTable("Estancias");

        builder.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Propietario).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Representante).HasMaxLength(200);
        builder.Property(e => e.Telefono).HasMaxLength(30);
        builder.Property(e => e.Renspa).HasMaxLength(50);
        builder.Property(e => e.Departamento).HasMaxLength(100);
        builder.Property(e => e.Provincia).HasMaxLength(100);
        builder.Property(e => e.Municipio).HasMaxLength(100);
        builder.Property(e => e.EstadoSync).HasConversion<string>().HasMaxLength(20);

        // Composición del documento: al eliminar la estancia, sus captaciones dejan de existir.
        builder.HasMany(e => e.Captaciones)
            .WithOne(c => c.Estancia)
            .HasForeignKey(c => c.EstanciaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

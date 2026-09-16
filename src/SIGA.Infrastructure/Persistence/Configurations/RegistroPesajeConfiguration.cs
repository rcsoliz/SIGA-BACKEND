using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence.Configurations;

public class RegistroPesajeConfiguration : IEntityTypeConfiguration<RegistroPesaje>
{
    public void Configure(EntityTypeBuilder<RegistroPesaje> builder)
    {
        builder.ToTable("RegistrosPesaje", t =>
        {
            t.HasCheckConstraint("CK_RegistrosPesaje_PesoPromedioKg", "\"PesoPromedioKg\" > 0");
            t.HasCheckConstraint("CK_RegistrosPesaje_CantidadCabezasPesadas", "\"CantidadCabezasPesadas\" IS NULL OR \"CantidadCabezasPesadas\" > 0");
        });

        builder.Property(r => r.EstadoSync).HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.Observaciones).HasMaxLength(1000);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(r => r.CreadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(r => r.ModificadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

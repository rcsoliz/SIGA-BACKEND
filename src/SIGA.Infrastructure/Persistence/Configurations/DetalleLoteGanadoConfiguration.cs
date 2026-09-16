using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence.Configurations;

public class DetalleLoteGanadoConfiguration : IEntityTypeConfiguration<DetalleLoteGanado>
{
    public void Configure(EntityTypeBuilder<DetalleLoteGanado> builder)
    {
        builder.ToTable("DetallesLoteGanado", t =>
        {
            t.HasCheckConstraint("CK_DetallesLoteGanado_CantidadCabezas", "\"CantidadCabezas\" > 0");
            t.HasCheckConstraint("CK_DetallesLoteGanado_PesoPromedioEstimadoKg", "\"PesoPromedioEstimadoKg\" IS NULL OR \"PesoPromedioEstimadoKg\" > 0");
        });

        builder.Property(d => d.Categoria).HasConversion<string>().HasMaxLength(20);
        builder.Property(d => d.Raza).HasMaxLength(100);
        builder.Property(d => d.SistemaAlimentacion).HasConversion<string>().HasMaxLength(30);
        builder.Property(d => d.NotasZootecnicas).HasMaxLength(1000);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(d => d.CreadoPor)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(d => d.ActualizadoPor)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.RazaCatalogo)
            .WithMany()
            .HasForeignKey(d => d.RazaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

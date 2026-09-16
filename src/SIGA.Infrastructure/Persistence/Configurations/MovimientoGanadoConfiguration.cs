using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence.Configurations;

public class MovimientoGanadoConfiguration : IEntityTypeConfiguration<MovimientoGanado>
{
    public void Configure(EntityTypeBuilder<MovimientoGanado> builder)
    {
        builder.ToTable("MovimientosGanado", t =>
        {
            t.HasCheckConstraint("CK_MovimientosGanado_CantidadCabezas", "\"CantidadCabezas\" > 0");
        });

        builder.Property(m => m.TipoGanado).HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.EstadoSync).HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.Origen).IsRequired().HasMaxLength(150);
        builder.Property(m => m.Destino).IsRequired().HasMaxLength(150);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(m => m.CreadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(m => m.ModificadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

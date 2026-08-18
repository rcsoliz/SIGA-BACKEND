using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence.Configurations;

public class MovimientoGanadoConfiguration : IEntityTypeConfiguration<MovimientoGanado>
{
    public void Configure(EntityTypeBuilder<MovimientoGanado> builder)
    {
        builder.ToTable("MovimientosGanado");

        builder.Property(m => m.TipoGanado).HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.EstadoSync).HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.Origen).IsRequired().HasMaxLength(150);
        builder.Property(m => m.Destino).IsRequired().HasMaxLength(150);
    }
}

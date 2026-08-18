using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence.Configurations;

public class DetalleLoteGanadoConfiguration : IEntityTypeConfiguration<DetalleLoteGanado>
{
    public void Configure(EntityTypeBuilder<DetalleLoteGanado> builder)
    {
        builder.ToTable("DetallesLoteGanado");

        builder.Property(d => d.Categoria).HasConversion<string>().HasMaxLength(20);
        builder.Property(d => d.Raza).HasMaxLength(100);
        builder.Property(d => d.SistemaAlimentacion).HasConversion<string>().HasMaxLength(30);
        builder.Property(d => d.NotasZootecnicas).HasMaxLength(1000);
    }
}

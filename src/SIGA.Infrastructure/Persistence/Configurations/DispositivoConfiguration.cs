using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence.Configurations;

public class DispositivoConfiguration : IEntityTypeConfiguration<Dispositivo>
{
    public void Configure(EntityTypeBuilder<Dispositivo> builder)
    {
        builder.ToTable("Dispositivos");
        builder.Property(d => d.IdentificadorDispositivo).IsRequired().HasMaxLength(150);
        builder.Property(d => d.UbicacionActual).HasMaxLength(150);
        builder.Property(d => d.Estado).HasConversion<string>().HasMaxLength(20);
    }
}

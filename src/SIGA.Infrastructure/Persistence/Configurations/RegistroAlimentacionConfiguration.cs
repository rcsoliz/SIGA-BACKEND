using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence.Configurations;

public class RegistroAlimentacionConfiguration : IEntityTypeConfiguration<RegistroAlimentacion>
{
    public void Configure(EntityTypeBuilder<RegistroAlimentacion> builder)
    {
        builder.ToTable("RegistrosAlimentacion");

        builder.Property(r => r.TipoAlimentacion).HasConversion<string>().HasMaxLength(30);
        builder.Property(r => r.EstadoSync).HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.Observaciones).HasMaxLength(1000);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence.Configurations;

public class CaptacionGanadoConfiguration : IEntityTypeConfiguration<CaptacionGanado>
{
    public void Configure(EntityTypeBuilder<CaptacionGanado> builder)
    {
        builder.ToTable("CaptacionesGanado");

        builder.Property(c => c.Nombre).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Observaciones).HasMaxLength(1000);
        builder.Property(c => c.Potrero).HasMaxLength(150);
        builder.Property(c => c.Estado).HasConversion<string>().HasMaxLength(30);
        builder.Property(c => c.EstadoSanitario).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.EstadoSync).HasConversion<string>().HasMaxLength(20);

        builder.HasMany(c => c.Detalles)
            .WithOne(d => d.CaptacionGanado)
            .HasForeignKey(d => d.CaptacionGanadoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.RegistrosAlimentacion)
            .WithOne(r => r.CaptacionGanado)
            .HasForeignKey(r => r.CaptacionGanadoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.RegistrosSanitarios)
            .WithOne(r => r.CaptacionGanado)
            .HasForeignKey(r => r.CaptacionGanadoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Movimientos)
            .WithOne(m => m.CaptacionGanado)
            .HasForeignKey(m => m.CaptacionGanadoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

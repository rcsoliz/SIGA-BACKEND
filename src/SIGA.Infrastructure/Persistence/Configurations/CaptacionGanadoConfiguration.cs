using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence.Configurations;

public class CaptacionGanadoConfiguration : IEntityTypeConfiguration<CaptacionGanado>
{
    public void Configure(EntityTypeBuilder<CaptacionGanado> builder)
    {
        builder.ToTable("CaptacionesGanado", t =>
        {
            t.HasCheckConstraint("CK_CaptacionesGanado_Latitud", "\"Latitud\" IS NULL OR \"Latitud\" BETWEEN -90 AND 90");
            t.HasCheckConstraint("CK_CaptacionesGanado_Longitud", "\"Longitud\" IS NULL OR \"Longitud\" BETWEEN -180 AND 180");
        });

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

        builder.HasMany(c => c.RegistrosPesaje)
            .WithOne(r => r.CaptacionGanado)
            .HasForeignKey(r => r.CaptacionGanadoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(c => c.CreadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(c => c.ModificadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

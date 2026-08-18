using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence.Configurations;

public class RegistroSanitarioConfiguration : IEntityTypeConfiguration<RegistroSanitario>
{
    public void Configure(EntityTypeBuilder<RegistroSanitario> builder)
    {
        builder.ToTable("RegistrosSanitarios");

        builder.Property(r => r.TipoEvento).HasConversion<string>().HasMaxLength(30);
        builder.Property(r => r.EstadoSync).HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.ProductoTratamiento).HasMaxLength(200);
        builder.Property(r => r.Observaciones).HasMaxLength(1000);

        // Distinta de CreadoPorUsuarioId (captura offline): quién realizó el tratamiento.
        builder.HasOne(r => r.RegistradoPor)
            .WithMany()
            .HasForeignKey(r => r.RegistradoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

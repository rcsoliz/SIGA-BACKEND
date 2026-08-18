using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence.Configurations;

public class LogAuditoriaConfiguration : IEntityTypeConfiguration<LogAuditoria>
{
    public void Configure(EntityTypeBuilder<LogAuditoria> builder)
    {
        builder.ToTable("LogsAuditoria");
        builder.Property(l => l.Accion).HasConversion<string>().HasMaxLength(20);
        builder.Property(l => l.Modulo).IsRequired().HasMaxLength(100);
        builder.Property(l => l.Detalle).HasMaxLength(500);

        builder.HasOne(l => l.Usuario)
            .WithMany()
            .HasForeignKey(l => l.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(l => l.FechaHora);
    }
}

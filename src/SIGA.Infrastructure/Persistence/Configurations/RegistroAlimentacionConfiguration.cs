using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence.Configurations;

public class RegistroAlimentacionConfiguration : IEntityTypeConfiguration<RegistroAlimentacion>
{
    public void Configure(EntityTypeBuilder<RegistroAlimentacion> builder)
    {
        builder.ToTable("RegistrosAlimentacion", t =>
        {
            t.HasCheckConstraint("CK_RegistrosAlimentacion_RacionBaseKgAnimal", "\"RacionBaseKgAnimal\" IS NULL OR \"RacionBaseKgAnimal\" >= 0");
            t.HasCheckConstraint("CK_RegistrosAlimentacion_SuplementoProteicoKgAnimal", "\"SuplementoProteicoKgAnimal\" IS NULL OR \"SuplementoProteicoKgAnimal\" >= 0");
        });

        builder.Property(r => r.TipoAlimentacion).HasConversion<string>().HasMaxLength(30);
        builder.Property(r => r.EstadoSync).HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.Observaciones).HasMaxLength(1000);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(r => r.CreadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(r => r.ModificadoPorUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

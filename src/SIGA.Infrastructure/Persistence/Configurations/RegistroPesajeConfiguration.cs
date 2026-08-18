using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence.Configurations;

public class RegistroPesajeConfiguration : IEntityTypeConfiguration<RegistroPesaje>
{
    public void Configure(EntityTypeBuilder<RegistroPesaje> builder)
    {
        builder.ToTable("RegistrosPesaje");

        builder.Property(r => r.EstadoSync).HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.Observaciones).HasMaxLength(1000);
    }
}

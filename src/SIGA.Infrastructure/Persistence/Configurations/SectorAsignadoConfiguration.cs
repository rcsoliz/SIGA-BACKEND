using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence.Configurations;

public class SectorAsignadoConfiguration : IEntityTypeConfiguration<SectorAsignado>
{
    public void Configure(EntityTypeBuilder<SectorAsignado> builder)
    {
        builder.ToTable("SectoresAsignados");
        builder.Property(s => s.NombreSector).IsRequired().HasMaxLength(150);
        builder.Property(s => s.Zona).HasMaxLength(150);
    }
}

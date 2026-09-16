using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence.Configurations;

public class ProductoTratamientoConfiguration : IEntityTypeConfiguration<ProductoTratamiento>
{
    public void Configure(EntityTypeBuilder<ProductoTratamiento> builder)
    {
        builder.ToTable("ProductosTratamiento");

        builder.Property(p => p.Nombre).IsRequired().HasMaxLength(200);
        builder.HasIndex(p => p.Nombre).IsUnique();
    }
}

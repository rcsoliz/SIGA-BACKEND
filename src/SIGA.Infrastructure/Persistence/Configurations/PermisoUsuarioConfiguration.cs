using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence.Configurations;

public class PermisoUsuarioConfiguration : IEntityTypeConfiguration<PermisoUsuario>
{
    public void Configure(EntityTypeBuilder<PermisoUsuario> builder)
    {
        builder.ToTable("PermisosUsuario");
        builder.Property(p => p.TipoPermiso).HasConversion<string>().HasMaxLength(30);
    }
}

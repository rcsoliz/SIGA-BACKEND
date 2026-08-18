using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasDiscriminator<string>("TipoUsuario")
            .HasValue<Captador>("Captador")
            .HasValue<Administrador>("Administrador");

        builder.Property(u => u.Nombre).IsRequired().HasMaxLength(150);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(200);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.Cargo).HasMaxLength(150);
        builder.Property(u => u.Rol).HasConversion<string>().HasMaxLength(30);
        builder.Property(u => u.Estado).HasConversion<string>().HasMaxLength(30);

        builder.HasMany(u => u.SectoresAsignados)
            .WithOne(s => s.Usuario)
            .HasForeignKey(s => s.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Dispositivos)
            .WithOne(d => d.Usuario)
            .HasForeignKey(d => d.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Permisos)
            .WithOne(p => p.Usuario)
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

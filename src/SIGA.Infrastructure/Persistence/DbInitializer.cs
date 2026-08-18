using Microsoft.EntityFrameworkCore;
using SIGA.Application.Interfaces;
using SIGA.Domain.Entities;
using SIGA.Domain.Enums;

namespace SIGA.Infrastructure.Persistence;

/// <summary>
/// Datos semilla para desarrollo local: un Administrador y un Captador de ejemplo para
/// poder probar login y flujos de campo desde el frontend sin crear usuarios a mano.
/// </summary>
public static class DbInitializer
{
    public static async Task SeedAsync(SigaDbContext context, IPasswordHasher passwordHasher)
    {
        await context.Database.MigrateAsync();

        if (await context.Usuarios.AnyAsync())
        {
            return;
        }

        var admin = new Administrador
        {
            Nombre = "Admin SIGA",
            Email = "admin@siga.com",
            Cargo = "Administrador del Sistema",
            PasswordHash = passwordHasher.Hash("Admin123!"),
            Estado = EstadoUsuario.Activo
        };

        var captador = new Captador
        {
            Nombre = "Juan Pérez",
            Email = "captador@siga.com",
            Cargo = "Captador de Campo",
            PasswordHash = passwordHasher.Hash("Captador123!"),
            Estado = EstadoUsuario.Activo
        };

        context.Usuarios.AddRange(admin, captador);
        await context.SaveChangesAsync();
    }
}

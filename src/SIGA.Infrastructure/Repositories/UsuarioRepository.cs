using Microsoft.EntityFrameworkCore;
using SIGA.Application.Interfaces;
using SIGA.Domain.Entities;
using SIGA.Infrastructure.Persistence;

namespace SIGA.Infrastructure.Repositories;

public class UsuarioRepository(SigaDbContext context) : Repository<Usuario>(context), IUsuarioRepository
{
    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        await DbSet.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default) =>
        await DbSet.AnyAsync(u => u.Email == email, ct);

    public async Task<Usuario?> GetConDetalleAsync(Guid id, CancellationToken ct = default) =>
        await DbSet
            .Include(u => u.SectoresAsignados)
            .Include(u => u.Dispositivos)
            .Include(u => u.Permisos)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

    // Se agregan directo al DbSet (no solo a usuario.SectoresAsignados/.Permisos): el Id de
    // BaseEntity ya viene generado en cliente, así que si dependiéramos del fixup automático
    // de la colección de un Usuario ya tracked, EF asumiría que la fila ya existe y emitiría
    // un UPDATE en vez de un INSERT (DbUpdateConcurrencyException, 0 filas afectadas).
    public async Task AgregarSectorAsync(SectorAsignado sector, CancellationToken ct = default) =>
        await Context.Set<SectorAsignado>().AddAsync(sector, ct);

    public async Task AgregarPermisoAsync(PermisoUsuario permiso, CancellationToken ct = default) =>
        await Context.Set<PermisoUsuario>().AddAsync(permiso, ct);
}

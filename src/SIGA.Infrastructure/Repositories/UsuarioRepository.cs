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
}

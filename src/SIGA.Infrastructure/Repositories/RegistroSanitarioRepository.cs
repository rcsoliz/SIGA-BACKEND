using Microsoft.EntityFrameworkCore;
using SIGA.Application.Interfaces;
using SIGA.Domain.Entities;
using SIGA.Infrastructure.Persistence;

namespace SIGA.Infrastructure.Repositories;

public class RegistroSanitarioRepository(SigaDbContext context)
    : Repository<RegistroSanitario>(context), IRegistroSanitarioRepository
{
    public override async Task<RegistroSanitario?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await DbSet.Include(r => r.RegistradoPor).FirstOrDefaultAsync(r => r.Id == id, ct);

    public override async Task<IReadOnlyList<RegistroSanitario>> GetAllAsync(CancellationToken ct = default) =>
        await DbSet.Include(r => r.CaptacionGanado).Include(r => r.RegistradoPor).ToListAsync(ct);

    public async Task<IReadOnlyList<RegistroSanitario>> GetByCaptacionAsync(Guid captacionId, CancellationToken ct = default) =>
        await DbSet.Include(r => r.RegistradoPor)
            .Where(r => r.CaptacionGanadoId == captacionId)
            .OrderByDescending(r => r.Fecha)
            .ToListAsync(ct);
}

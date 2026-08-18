using Microsoft.EntityFrameworkCore;
using SIGA.Application.Interfaces;
using SIGA.Domain.Entities;
using SIGA.Infrastructure.Persistence;

namespace SIGA.Infrastructure.Repositories;

public class CaptacionGanadoRepository(SigaDbContext context)
    : Repository<CaptacionGanado>(context), ICaptacionGanadoRepository
{
    public override async Task<CaptacionGanado?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await DbSet.Include(c => c.Detalles).FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<CaptacionGanado>> GetByEstanciaAsync(Guid estanciaId, CancellationToken ct = default) =>
        await DbSet.Include(c => c.Detalles).Where(c => c.EstanciaId == estanciaId).ToListAsync(ct);

    public async Task<CaptacionGanado?> GetConDetallesAsync(Guid id, CancellationToken ct = default) =>
        await DbSet.Include(c => c.Detalles).FirstOrDefaultAsync(c => c.Id == id, ct);
}

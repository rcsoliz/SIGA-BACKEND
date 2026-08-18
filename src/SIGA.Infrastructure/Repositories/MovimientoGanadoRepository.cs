using Microsoft.EntityFrameworkCore;
using SIGA.Application.Interfaces;
using SIGA.Domain.Entities;
using SIGA.Infrastructure.Persistence;

namespace SIGA.Infrastructure.Repositories;

public class MovimientoGanadoRepository(SigaDbContext context)
    : Repository<MovimientoGanado>(context), IMovimientoGanadoRepository
{
    public override async Task<IReadOnlyList<MovimientoGanado>> GetAllAsync(CancellationToken ct = default) =>
        await DbSet.Include(m => m.CaptacionGanado).ThenInclude(c => c.Estancia).ThenInclude(e => e.Captador).ToListAsync(ct);

    public async Task<IReadOnlyList<MovimientoGanado>> GetByCaptacionAsync(Guid captacionId, CancellationToken ct = default) =>
        await DbSet.Where(m => m.CaptacionGanadoId == captacionId).OrderByDescending(m => m.Fecha).ToListAsync(ct);
}

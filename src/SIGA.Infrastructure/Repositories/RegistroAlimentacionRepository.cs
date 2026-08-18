using Microsoft.EntityFrameworkCore;
using SIGA.Application.Interfaces;
using SIGA.Domain.Entities;
using SIGA.Infrastructure.Persistence;

namespace SIGA.Infrastructure.Repositories;

public class RegistroAlimentacionRepository(SigaDbContext context)
    : Repository<RegistroAlimentacion>(context), IRegistroAlimentacionRepository
{
    public override async Task<IReadOnlyList<RegistroAlimentacion>> GetAllAsync(CancellationToken ct = default) =>
        await DbSet.Include(r => r.CaptacionGanado).ThenInclude(c => c.Estancia).ThenInclude(e => e.Captador).ToListAsync(ct);

    public async Task<IReadOnlyList<RegistroAlimentacion>> GetByCaptacionAsync(Guid captacionId, CancellationToken ct = default) =>
        await DbSet.Where(r => r.CaptacionGanadoId == captacionId).OrderByDescending(r => r.Fecha).ToListAsync(ct);
}

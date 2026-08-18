using Microsoft.EntityFrameworkCore;
using SIGA.Application.Interfaces;
using SIGA.Domain.Entities;
using SIGA.Infrastructure.Persistence;

namespace SIGA.Infrastructure.Repositories;

public class RegistroPesajeRepository(SigaDbContext context)
    : Repository<RegistroPesaje>(context), IRegistroPesajeRepository
{
    public override async Task<IReadOnlyList<RegistroPesaje>> GetAllAsync(CancellationToken ct = default) =>
        await DbSet.Include(r => r.CaptacionGanado).ThenInclude(c => c.Estancia).ThenInclude(e => e.Captador).ToListAsync(ct);

    public async Task<IReadOnlyList<RegistroPesaje>> GetByCaptacionAsync(Guid captacionId, CancellationToken ct = default) =>
        await DbSet.Where(r => r.CaptacionGanadoId == captacionId).OrderByDescending(r => r.Fecha).ToListAsync(ct);
}

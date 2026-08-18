using Microsoft.EntityFrameworkCore;
using SIGA.Application.Interfaces;
using SIGA.Domain.Entities;
using SIGA.Infrastructure.Persistence;

namespace SIGA.Infrastructure.Repositories;

public class EstanciaRepository(SigaDbContext context) : Repository<Estancia>(context), IEstanciaRepository
{
    public override async Task<Estancia?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await DbSet.Include(e => e.Captaciones).ThenInclude(c => c.Detalles).FirstOrDefaultAsync(e => e.Id == id, ct);

    public override async Task<IReadOnlyList<Estancia>> GetAllAsync(CancellationToken ct = default) =>
        await DbSet.Include(e => e.Captaciones).ThenInclude(c => c.Detalles).ToListAsync(ct);

    public async Task<IReadOnlyList<Estancia>> GetByCaptadorAsync(Guid captadorId, CancellationToken ct = default) =>
        await DbSet.Include(e => e.Captaciones).ThenInclude(c => c.Detalles)
            .Where(e => e.CaptadorId == captadorId).ToListAsync(ct);

    public async Task<Estancia?> GetWithCaptacionesAsync(Guid id, CancellationToken ct = default) =>
        await DbSet.Include(e => e.Captaciones).ThenInclude(c => c.Detalles).FirstOrDefaultAsync(e => e.Id == id, ct);
}

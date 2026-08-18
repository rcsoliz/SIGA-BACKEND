using Microsoft.EntityFrameworkCore;
using SIGA.Application.Interfaces;
using SIGA.Domain.Entities;
using SIGA.Infrastructure.Persistence;

namespace SIGA.Infrastructure.Repositories;

public class LogAuditoriaRepository(SigaDbContext context)
    : Repository<LogAuditoria>(context), ILogAuditoriaRepository
{
    public async Task<IReadOnlyList<LogAuditoria>> BuscarAsync(
        DateTime? desde,
        DateTime? hasta,
        Guid? usuarioId,
        string? modulo,
        CancellationToken ct = default)
    {
        var query = DbSet.Include(l => l.Usuario).AsQueryable();

        if (desde is not null) query = query.Where(l => l.FechaHora >= desde);
        if (hasta is not null) query = query.Where(l => l.FechaHora <= hasta);
        if (usuarioId is not null) query = query.Where(l => l.UsuarioId == usuarioId);
        if (!string.IsNullOrWhiteSpace(modulo)) query = query.Where(l => l.Modulo == modulo);

        return await query.OrderByDescending(l => l.FechaHora).ToListAsync(ct);
    }
}

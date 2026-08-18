using SIGA.Domain.Entities;

namespace SIGA.Application.Interfaces;

public interface IEstanciaRepository : IRepository<Estancia>
{
    Task<IReadOnlyList<Estancia>> GetByCaptadorAsync(Guid captadorId, CancellationToken ct = default);
    Task<Estancia?> GetWithCaptacionesAsync(Guid id, CancellationToken ct = default);
}

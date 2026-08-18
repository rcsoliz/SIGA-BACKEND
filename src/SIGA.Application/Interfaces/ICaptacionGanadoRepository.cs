using SIGA.Domain.Entities;

namespace SIGA.Application.Interfaces;

public interface ICaptacionGanadoRepository : IRepository<CaptacionGanado>
{
    Task<IReadOnlyList<CaptacionGanado>> GetByEstanciaAsync(Guid estanciaId, CancellationToken ct = default);

    /// <summary>Carga la cabecera junto con su colección de detalle (para agregar/quitar líneas).</summary>
    Task<CaptacionGanado?> GetConDetallesAsync(Guid id, CancellationToken ct = default);
}

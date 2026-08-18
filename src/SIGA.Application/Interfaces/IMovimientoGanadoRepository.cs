using SIGA.Domain.Entities;

namespace SIGA.Application.Interfaces;

public interface IMovimientoGanadoRepository : IRepository<MovimientoGanado>
{
    Task<IReadOnlyList<MovimientoGanado>> GetByCaptacionAsync(Guid captacionId, CancellationToken ct = default);
}

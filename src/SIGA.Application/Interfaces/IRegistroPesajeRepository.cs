using SIGA.Domain.Entities;

namespace SIGA.Application.Interfaces;

public interface IRegistroPesajeRepository : IRepository<RegistroPesaje>
{
    Task<IReadOnlyList<RegistroPesaje>> GetByCaptacionAsync(Guid captacionId, CancellationToken ct = default);
}

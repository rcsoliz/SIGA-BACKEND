using SIGA.Domain.Entities;

namespace SIGA.Application.Interfaces;

public interface IRegistroAlimentacionRepository : IRepository<RegistroAlimentacion>
{
    Task<IReadOnlyList<RegistroAlimentacion>> GetByCaptacionAsync(Guid captacionId, CancellationToken ct = default);
}

using SIGA.Domain.Entities;

namespace SIGA.Application.Interfaces;

public interface IRegistroSanitarioRepository : IRepository<RegistroSanitario>
{
    Task<IReadOnlyList<RegistroSanitario>> GetByCaptacionAsync(Guid captacionId, CancellationToken ct = default);
}

using SIGA.Domain.Entities;

namespace SIGA.Application.Interfaces;

public interface ILogAuditoriaRepository : IRepository<LogAuditoria>
{
    Task<IReadOnlyList<LogAuditoria>> BuscarAsync(
        DateTime? desde,
        DateTime? hasta,
        Guid? usuarioId,
        string? modulo,
        CancellationToken ct = default);
}

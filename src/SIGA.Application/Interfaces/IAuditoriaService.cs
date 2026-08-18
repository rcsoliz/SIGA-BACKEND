using SIGA.Application.DTOs.Auditoria;
using SIGA.Domain.Enums;

namespace SIGA.Application.Interfaces;

public interface IAuditoriaService
{
    Task RegistrarAsync(
        AccionAuditoria accion,
        string modulo,
        Guid idRegistroAfectado,
        string? detalle = null,
        CancellationToken ct = default);

    Task<IReadOnlyList<LogAuditoriaDto>> BuscarAsync(BuscarAuditoriaQuery query, CancellationToken ct = default);
}

using SIGA.Application.DTOs.Auditoria;
using SIGA.Application.Interfaces;
using SIGA.Domain.Entities;
using SIGA.Domain.Enums;

namespace SIGA.Application.Services;

public class AuditoriaService(
    ILogAuditoriaRepository logAuditoriaRepository,
    ICurrentUserService currentUserService) : IAuditoriaService
{
    public async Task RegistrarAsync(
        AccionAuditoria accion,
        string modulo,
        Guid idRegistroAfectado,
        string? detalle = null,
        CancellationToken ct = default)
    {
        if (currentUserService.UsuarioId is not { } usuarioId)
        {
            // No hay usuario autenticado en el contexto (ej. seed/tareas de sistema): no se audita.
            return;
        }

        var log = new LogAuditoria
        {
            UsuarioId = usuarioId,
            Accion = accion,
            Modulo = modulo,
            IdRegistroAfectado = idRegistroAfectado,
            Detalle = detalle,
            FechaHora = DateTime.UtcNow
        };

        await logAuditoriaRepository.AddAsync(log, ct);
        await logAuditoriaRepository.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<LogAuditoriaDto>> BuscarAsync(BuscarAuditoriaQuery query, CancellationToken ct = default)
    {
        var logs = await logAuditoriaRepository.BuscarAsync(query.Desde, query.Hasta, query.UsuarioId, query.Modulo, ct);

        return logs.Select(l => new LogAuditoriaDto(
            l.Id,
            l.UsuarioId,
            l.Usuario?.Nombre ?? string.Empty,
            l.Accion.ToString(),
            l.Modulo,
            l.IdRegistroAfectado,
            l.FechaHora,
            l.Detalle)).ToList();
    }
}

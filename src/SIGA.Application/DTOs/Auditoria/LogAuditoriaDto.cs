namespace SIGA.Application.DTOs.Auditoria;

public record LogAuditoriaDto(
    Guid Id,
    Guid UsuarioId,
    string UsuarioNombre,
    string Accion,
    string Modulo,
    Guid IdRegistroAfectado,
    DateTime FechaHora,
    string? Detalle);

public record BuscarAuditoriaQuery(
    DateTime? Desde,
    DateTime? Hasta,
    Guid? UsuarioId,
    string? Modulo);

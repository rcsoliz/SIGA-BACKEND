namespace SIGA.Application.DTOs.Sanitario;

public record RegistroSanitarioDto(
    Guid Id,
    Guid CaptacionGanadoId,
    DateTime Fecha,
    string TipoEvento,
    string? ProductoTratamiento,
    Guid RegistradoPorUsuarioId,
    string RegistradoPorNombre,
    string? Observaciones,
    string EstadoSync);

public record CreateRegistroSanitarioDto(
    Guid CaptacionGanadoId,
    DateTime Fecha,
    string TipoEvento,
    string? ProductoTratamiento,
    string? Observaciones,
    DateTime FechaCreacionLocal);

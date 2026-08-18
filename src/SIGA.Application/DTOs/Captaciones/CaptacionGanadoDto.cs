namespace SIGA.Application.DTOs.Captaciones;

public record CaptacionGanadoDto(
    Guid Id,
    Guid EstanciaId,
    string Nombre,
    string? Observaciones,
    string Estado,
    string EstadoSanitario,
    string? Potrero,
    DateTime Fecha,
    double? Latitud,
    double? Longitud,
    string EstadoSync,
    int TotalCabezas,
    double PesoEstimadoTotal,
    int? DiasEnPotrero,
    IReadOnlyList<DetalleLoteGanadoDto> Detalles);

public record CreateCaptacionGanadoDto(
    Guid EstanciaId,
    string Nombre,
    string? Observaciones,
    string? Potrero,
    DateTime Fecha,
    double? Latitud,
    double? Longitud,
    DateTime FechaCreacionLocal,
    IReadOnlyList<CreateDetalleLoteGanadoDto> Detalles);

public record UpdateCaptacionGanadoDto(
    string Nombre,
    string? Observaciones,
    string Estado,
    string EstadoSanitario,
    string? Potrero);

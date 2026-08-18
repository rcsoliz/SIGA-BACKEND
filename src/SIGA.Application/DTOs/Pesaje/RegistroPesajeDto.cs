namespace SIGA.Application.DTOs.Pesaje;

public record RegistroPesajeDto(
    Guid Id,
    Guid CaptacionGanadoId,
    DateTime Fecha,
    double PesoPromedioKg,
    int? CantidadCabezasPesadas,
    string? Observaciones,
    string EstadoSync);

public record CreateRegistroPesajeDto(
    Guid CaptacionGanadoId,
    DateTime Fecha,
    double PesoPromedioKg,
    int? CantidadCabezasPesadas,
    string? Observaciones,
    DateTime FechaCreacionLocal);

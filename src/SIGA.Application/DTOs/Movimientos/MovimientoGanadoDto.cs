namespace SIGA.Application.DTOs.Movimientos;

public record MovimientoGanadoDto(
    Guid Id,
    Guid CaptacionGanadoId,
    DateTime Fecha,
    string TipoGanado,
    int CantidadCabezas,
    string Origen,
    string Destino,
    string EstadoSync);

public record CreateMovimientoGanadoDto(
    Guid CaptacionGanadoId,
    DateTime Fecha,
    string TipoGanado,
    int CantidadCabezas,
    string Origen,
    string Destino,
    DateTime FechaCreacionLocal);

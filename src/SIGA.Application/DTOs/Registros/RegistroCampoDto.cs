namespace SIGA.Application.DTOs.Registros;

/// <summary>
/// Vista unificada de las tres bitácoras (Movimiento, Alimentación, Sanitario) para la
/// pantalla "Maestro de Registros". No es una entidad propia: se arma por consulta.
/// </summary>
public record RegistroCampoDto(
    Guid Id,
    DateTime FechaHora,
    string Tipo,
    Guid CaptacionGanadoId,
    string CaptacionNombre,
    string DetalleMetrica,
    string? ProductoTratamiento,
    string RegistradoPor,
    string EstadoSync);

public record BuscarRegistrosQuery(
    DateTime? Desde,
    DateTime? Hasta,
    string? Tipo,
    Guid? CaptacionGanadoId,
    string? Texto);

namespace SIGA.Application.DTOs.Usuarios;

public record DispositivoDto(
    Guid Id,
    string IdentificadorDispositivo,
    DateTime? UltimaSincronizacion,
    string? UbicacionActual,
    string Estado);

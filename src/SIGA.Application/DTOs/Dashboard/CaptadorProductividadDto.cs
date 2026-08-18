namespace SIGA.Application.DTOs.Dashboard;

/// <summary>Fila de la tabla de ranking (GET /api/dashboard/captadores).</summary>
public record CaptadorRankingDto(
    Guid UsuarioId,
    string Nombre,
    string? Cargo,
    int EstanciasRegistradas,
    int CaptacionesRegistradas,
    int CaptacionesActivas,
    int TotalCabezasCapturadas);

/// <summary>Perfil completo de un captador (GET /api/dashboard/captadores/{id}).</summary>
public record CaptadorProductividadDto(
    Guid UsuarioId,
    string Nombre,
    string? Cargo,
    string Estado,
    IReadOnlyList<string> SectoresAsignados,
    int EstanciasRegistradas,
    int CaptacionesRegistradas,
    int CaptacionesActivas,
    int TotalCabezasCapturadas,
    DateTime? UltimaActividad);

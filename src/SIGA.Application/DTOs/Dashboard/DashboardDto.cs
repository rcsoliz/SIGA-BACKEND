namespace SIGA.Application.DTOs.Dashboard;

public record DashboardDto(
    int TotalCabezasActivas,
    int CaptacionesActivas,
    int CaptacionesPendientesRevision,
    IReadOnlyList<CabezasPorCategoriaDto> CabezasPorCategoria,
    IReadOnlyList<DashboardSerieMensualDto> SerieMensual);

/// <summary>Un punto del gráfico "Consumo vs Ganancia de Peso" (Dashboard de Gestión).</summary>
public record DashboardSerieMensualDto(
    string Mes,
    double? ConsumoPromedioKg,
    double? PesoPromedioKg);

/// <summary>Total de cabezas activas agrupado por categoría (Toro, Novillo, Vaquilla, ...).</summary>
public record CabezasPorCategoriaDto(
    string Categoria,
    int Cantidad);

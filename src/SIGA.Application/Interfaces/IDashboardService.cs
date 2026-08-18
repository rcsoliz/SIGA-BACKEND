using SIGA.Application.DTOs.Dashboard;

namespace SIGA.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> ObtenerResumenAsync(CancellationToken ct = default);

    Task<IReadOnlyList<CaptadorRankingDto>> ListarProductividadCaptadoresAsync(
        DateTime? desde, DateTime? hasta, CancellationToken ct = default);

    Task<CaptadorProductividadDto> ObtenerProductividadCaptadorAsync(
        Guid captadorId, DateTime? desde, DateTime? hasta, CancellationToken ct = default);
}

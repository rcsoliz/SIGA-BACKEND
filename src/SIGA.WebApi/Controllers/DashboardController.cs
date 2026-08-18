using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGA.Application.DTOs.Dashboard;
using SIGA.Application.Interfaces;

namespace SIGA.WebApi.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<DashboardDto>> ObtenerResumen(CancellationToken ct) =>
        Ok(await dashboardService.ObtenerResumenAsync(ct));

    /// <summary>Ranking de productividad de todos los captadores (solo Administrador).</summary>
    [HttpGet("captadores")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<IReadOnlyList<CaptadorRankingDto>>> ListarProductividadCaptadores(
        [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, CancellationToken ct) =>
        Ok(await dashboardService.ListarProductividadCaptadoresAsync(desde, hasta, ct));

    /// <summary>
    /// Perfil de productividad de un captador. Un Administrador puede consultar cualquiera;
    /// un Captador solo puede consultar el suyo (validado en el servicio).
    /// </summary>
    [HttpGet("captadores/{id:guid}")]
    public async Task<ActionResult<CaptadorProductividadDto>> ObtenerProductividadCaptador(
        Guid id, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, CancellationToken ct) =>
        Ok(await dashboardService.ObtenerProductividadCaptadorAsync(id, desde, hasta, ct));
}

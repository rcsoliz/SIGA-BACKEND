using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGA.Application.DTOs.Auditoria;
using SIGA.Application.Interfaces;

namespace SIGA.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class AuditoriaController(IAuditoriaService auditoriaService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LogAuditoriaDto>>> Buscar(
        [FromQuery] DateTime? desde,
        [FromQuery] DateTime? hasta,
        [FromQuery] Guid? usuarioId,
        [FromQuery] string? modulo,
        CancellationToken ct) =>
        Ok(await auditoriaService.BuscarAsync(new BuscarAuditoriaQuery(desde, hasta, usuarioId, modulo), ct));
}

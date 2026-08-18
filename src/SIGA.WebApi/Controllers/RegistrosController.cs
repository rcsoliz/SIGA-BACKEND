using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGA.Application.DTOs.Registros;
using SIGA.Application.Interfaces;

namespace SIGA.WebApi.Controllers;

/// <summary>
/// Maestro de Registros: vista unificada de las tres bitácoras de campo.
/// </summary>
[ApiController]
[Route("api/registros")]
[Authorize(Roles = "Administrador")]
public class RegistrosController(IRegistroCampoService registroCampoService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RegistroCampoDto>>> Buscar(
        [FromQuery] DateTime? desde,
        [FromQuery] DateTime? hasta,
        [FromQuery] string? tipo,
        [FromQuery] Guid? captacionId,
        [FromQuery] string? texto,
        CancellationToken ct) =>
        Ok(await registroCampoService.BuscarAsync(new BuscarRegistrosQuery(desde, hasta, tipo, captacionId, texto), ct));
}

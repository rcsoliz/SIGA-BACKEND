using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGA.Application.DTOs.Captaciones;
using SIGA.Application.Interfaces;

namespace SIGA.WebApi.Controllers;

/// <summary>
/// Cabecera de captura de campo (CaptacionGanado). Cada captación agrupa 1..* líneas de
/// detalle (DetalleLoteGanado), una por cada categoría de animal encontrada en la visita.
/// </summary>
[ApiController]
[Route("api/captaciones")]
[Authorize]
public class CaptacionesController(ICaptacionGanadoService captacionService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CaptacionGanadoDto>>> ListarPorEstancia(
        [FromQuery] Guid? estanciaId, CancellationToken ct) =>
        Ok(await captacionService.ListarPorEstanciaAsync(estanciaId, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CaptacionGanadoDto>> ObtenerPorId(Guid id, CancellationToken ct) =>
        Ok(await captacionService.ObtenerPorIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = "Captador")]
    public async Task<ActionResult<CaptacionGanadoDto>> Crear(CreateCaptacionGanadoDto dto, CancellationToken ct)
    {
        var captacion = await captacionService.CrearAsync(dto, ct);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = captacion.Id }, captacion);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CaptacionGanadoDto>> Actualizar(Guid id, UpdateCaptacionGanadoDto dto, CancellationToken ct) =>
        Ok(await captacionService.ActualizarAsync(id, dto, ct));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(Guid id, CancellationToken ct)
    {
        await captacionService.EliminarAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/detalles")]
    [Authorize(Roles = "Captador")]
    public async Task<ActionResult<DetalleLoteGanadoDto>> AgregarDetalle(
        Guid id, CreateDetalleLoteGanadoDto dto, CancellationToken ct) =>
        Ok(await captacionService.AgregarDetalleAsync(id, dto, ct));

    [HttpDelete("{id:guid}/detalles/{detalleId:guid}")]
    [Authorize(Roles = "Captador")]
    public async Task<IActionResult> EliminarDetalle(Guid id, Guid detalleId, CancellationToken ct)
    {
        await captacionService.EliminarDetalleAsync(id, detalleId, ct);
        return NoContent();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGA.Application.DTOs.Movimientos;
using SIGA.Application.Interfaces;

namespace SIGA.WebApi.Controllers;

[ApiController]
[Route("api/movimientos")]
[Authorize]
public class MovimientosController(IMovimientoGanadoService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MovimientoGanadoDto>>> ListarPorCaptacion(
        [FromQuery] Guid captacionId, CancellationToken ct) =>
        Ok(await service.ListarPorCaptacionAsync(captacionId, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MovimientoGanadoDto>> ObtenerPorId(Guid id, CancellationToken ct) =>
        Ok(await service.ObtenerPorIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = "Captador")]
    public async Task<ActionResult<MovimientoGanadoDto>> Crear(CreateMovimientoGanadoDto dto, CancellationToken ct) =>
        Ok(await service.CrearAsync(dto, ct));
}

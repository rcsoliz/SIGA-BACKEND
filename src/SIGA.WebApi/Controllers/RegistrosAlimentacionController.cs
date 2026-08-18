using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGA.Application.DTOs.Alimentacion;
using SIGA.Application.Interfaces;

namespace SIGA.WebApi.Controllers;

[ApiController]
[Route("api/registros-alimentacion")]
[Authorize]
public class RegistrosAlimentacionController(IRegistroAlimentacionService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RegistroAlimentacionDto>>> ListarPorCaptacion(
        [FromQuery] Guid captacionId, CancellationToken ct) =>
        Ok(await service.ListarPorCaptacionAsync(captacionId, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RegistroAlimentacionDto>> ObtenerPorId(Guid id, CancellationToken ct) =>
        Ok(await service.ObtenerPorIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = "Captador")]
    public async Task<ActionResult<RegistroAlimentacionDto>> Crear(CreateRegistroAlimentacionDto dto, CancellationToken ct) =>
        Ok(await service.CrearAsync(dto, ct));
}

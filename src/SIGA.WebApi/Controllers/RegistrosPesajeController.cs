using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGA.Application.DTOs.Pesaje;
using SIGA.Application.Interfaces;

namespace SIGA.WebApi.Controllers;

[ApiController]
[Route("api/registros-pesaje")]
[Authorize]
public class RegistrosPesajeController(IRegistroPesajeService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RegistroPesajeDto>>> ListarPorCaptacion(
        [FromQuery] Guid captacionId, CancellationToken ct) =>
        Ok(await service.ListarPorCaptacionAsync(captacionId, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RegistroPesajeDto>> ObtenerPorId(Guid id, CancellationToken ct) =>
        Ok(await service.ObtenerPorIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = "Captador")]
    public async Task<ActionResult<RegistroPesajeDto>> Crear(CreateRegistroPesajeDto dto, CancellationToken ct) =>
        Ok(await service.CrearAsync(dto, ct));
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGA.Application.DTOs.Sanitario;
using SIGA.Application.Interfaces;

namespace SIGA.WebApi.Controllers;

[ApiController]
[Route("api/registros-sanitarios")]
[Authorize]
public class RegistrosSanitariosController(IRegistroSanitarioService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RegistroSanitarioDto>>> ListarPorCaptacion(
        [FromQuery] Guid captacionId, CancellationToken ct) =>
        Ok(await service.ListarPorCaptacionAsync(captacionId, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RegistroSanitarioDto>> ObtenerPorId(Guid id, CancellationToken ct) =>
        Ok(await service.ObtenerPorIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<RegistroSanitarioDto>> Crear(CreateRegistroSanitarioDto dto, CancellationToken ct) =>
        Ok(await service.CrearAsync(dto, ct));
}

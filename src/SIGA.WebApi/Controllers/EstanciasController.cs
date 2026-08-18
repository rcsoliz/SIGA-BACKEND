using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGA.Application.DTOs.Estancias;
using SIGA.Application.Interfaces;

namespace SIGA.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EstanciasController(IEstanciaService estanciaService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EstanciaDto>>> Listar(CancellationToken ct) =>
        Ok(await estanciaService.ListarAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EstanciaDto>> ObtenerPorId(Guid id, CancellationToken ct) =>
        Ok(await estanciaService.ObtenerPorIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = "Captador")]
    public async Task<ActionResult<EstanciaDto>> Crear(CreateEstanciaDto dto, CancellationToken ct)
    {
        var estancia = await estanciaService.CrearAsync(dto, ct);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = estancia.Id }, estancia);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EstanciaDto>> Actualizar(Guid id, UpdateEstanciaDto dto, CancellationToken ct) =>
        Ok(await estanciaService.ActualizarAsync(id, dto, ct));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(Guid id, CancellationToken ct)
    {
        await estanciaService.EliminarAsync(id, ct);
        return NoContent();
    }
}

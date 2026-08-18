using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGA.Application.DTOs.Usuarios;
using SIGA.Application.Interfaces;

namespace SIGA.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class UsuariosController(IUsuarioService usuarioService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UsuarioDto>>> Listar(CancellationToken ct) =>
        Ok(await usuarioService.ListarAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UsuarioDetalleDto>> ObtenerDetalle(Guid id, CancellationToken ct) =>
        Ok(await usuarioService.ObtenerDetalleAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> Crear(CreateUsuarioDto dto, CancellationToken ct)
    {
        var usuario = await usuarioService.CrearAsync(dto, ct);
        return CreatedAtAction(nameof(ObtenerDetalle), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UsuarioDto>> Actualizar(Guid id, UpdateUsuarioDto dto, CancellationToken ct) =>
        Ok(await usuarioService.ActualizarAsync(id, dto, ct));

    [HttpPost("{id:guid}/sectores")]
    public async Task<ActionResult<SectorAsignadoDto>> AsignarSector(Guid id, CreateSectorAsignadoDto dto, CancellationToken ct) =>
        Ok(await usuarioService.AsignarSectorAsync(id, dto, ct));

    [HttpDelete("{id:guid}/sectores/{sectorId:guid}")]
    public async Task<IActionResult> QuitarSector(Guid id, Guid sectorId, CancellationToken ct)
    {
        await usuarioService.QuitarSectorAsync(id, sectorId, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/dispositivos/{dispositivoId:guid}/revocar")]
    public async Task<IActionResult> RevocarDispositivo(Guid id, Guid dispositivoId, CancellationToken ct)
    {
        await usuarioService.RevocarDispositivoAsync(id, dispositivoId, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/permisos")]
    public async Task<ActionResult<PermisoUsuarioDto>> AsignarPermiso(Guid id, AsignarPermisoDto dto, CancellationToken ct) =>
        Ok(await usuarioService.AsignarPermisoAsync(id, dto, ct));
}

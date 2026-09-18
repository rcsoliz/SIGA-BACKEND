using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGA.Application.DTOs.Catalogos;
using SIGA.Application.Interfaces;

namespace SIGA.WebApi.Controllers;

[ApiController]
[Route("api/catalogos")]
[Authorize]
public class CatalogosController(ICatalogoService catalogoService) : ControllerBase
{
    [HttpGet("departamentos")]
    public async Task<ActionResult<IReadOnlyList<DepartamentoDto>>> ListarDepartamentos(CancellationToken ct) =>
        Ok(await catalogoService.ListarDepartamentosAsync(ct));

    [HttpGet("departamentos/{id:guid}/provincias")]
    public async Task<ActionResult<IReadOnlyList<ProvinciaDto>>> ListarProvincias(Guid id, CancellationToken ct) =>
        Ok(await catalogoService.ListarProvinciasAsync(id, ct));

    [HttpGet("provincias/{id:guid}/municipios")]
    public async Task<ActionResult<IReadOnlyList<MunicipioDto>>> ListarMunicipios(Guid id, CancellationToken ct) =>
        Ok(await catalogoService.ListarMunicipiosAsync(id, ct));

    [HttpGet("razas")]
    public async Task<ActionResult<IReadOnlyList<RazaDto>>> ListarRazas(CancellationToken ct) =>
        Ok(await catalogoService.ListarRazasAsync(ct));

    [HttpGet("productos-tratamiento")]
    public async Task<ActionResult<IReadOnlyList<ProductoTratamientoDto>>> ListarProductos(CancellationToken ct) =>
        Ok(await catalogoService.ListarProductosTratamientoAsync(ct));
}

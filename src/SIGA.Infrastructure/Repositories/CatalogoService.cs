using Microsoft.EntityFrameworkCore;
using SIGA.Application.DTOs.Catalogos;
using SIGA.Application.Interfaces;
using SIGA.Infrastructure.Persistence;

namespace SIGA.Infrastructure.Repositories;

/// <summary>
/// Lectura simple de los catálogos normalizados (ubicación, raza, producto de
/// tratamiento) — sin lógica de negocio, por eso consulta el DbContext directo en vez de
/// pasar por el patrón Repository por entidad usado en el resto de Infrastructure.
/// </summary>
public class CatalogoService(SigaDbContext context) : ICatalogoService
{
    public async Task<IReadOnlyList<DepartamentoDto>> ListarDepartamentosAsync(CancellationToken ct = default) =>
        await context.Departamentos
            .OrderBy(d => d.Nombre)
            .Select(d => new DepartamentoDto(d.Id, d.Nombre))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ProvinciaDto>> ListarProvinciasAsync(Guid departamentoId, CancellationToken ct = default) =>
        await context.Provincias
            .Where(p => p.DepartamentoId == departamentoId)
            .OrderBy(p => p.Nombre)
            .Select(p => new ProvinciaDto(p.Id, p.Nombre, p.DepartamentoId))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<MunicipioDto>> ListarMunicipiosAsync(Guid provinciaId, CancellationToken ct = default) =>
        await context.Municipios
            .Where(m => m.ProvinciaId == provinciaId)
            .OrderBy(m => m.Nombre)
            .Select(m => new MunicipioDto(m.Id, m.Nombre, m.ProvinciaId))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<RazaDto>> ListarRazasAsync(CancellationToken ct = default) =>
        await context.Razas
            .OrderBy(r => r.Nombre)
            .Select(r => new RazaDto(r.Id, r.Nombre))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ProductoTratamientoDto>> ListarProductosTratamientoAsync(CancellationToken ct = default) =>
        await context.ProductosTratamiento
            .OrderBy(p => p.Nombre)
            .Select(p => new ProductoTratamientoDto(p.Id, p.Nombre))
            .ToListAsync(ct);
}

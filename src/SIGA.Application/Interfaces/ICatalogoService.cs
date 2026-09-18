using SIGA.Application.DTOs.Catalogos;

namespace SIGA.Application.Interfaces;

public interface ICatalogoService
{
    Task<IReadOnlyList<DepartamentoDto>> ListarDepartamentosAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ProvinciaDto>> ListarProvinciasAsync(Guid departamentoId, CancellationToken ct = default);
    Task<IReadOnlyList<MunicipioDto>> ListarMunicipiosAsync(Guid provinciaId, CancellationToken ct = default);
    Task<IReadOnlyList<RazaDto>> ListarRazasAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ProductoTratamientoDto>> ListarProductosTratamientoAsync(CancellationToken ct = default);
}

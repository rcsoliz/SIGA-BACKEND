using SIGA.Application.DTOs.Estancias;

namespace SIGA.Application.Interfaces;

public interface IEstanciaService
{
    Task<IReadOnlyList<EstanciaDto>> ListarAsync(CancellationToken ct = default);
    Task<EstanciaDto> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<EstanciaDto> CrearAsync(CreateEstanciaDto dto, CancellationToken ct = default);
    Task<EstanciaDto> ActualizarAsync(Guid id, UpdateEstanciaDto dto, CancellationToken ct = default);
    Task EliminarAsync(Guid id, CancellationToken ct = default);
}

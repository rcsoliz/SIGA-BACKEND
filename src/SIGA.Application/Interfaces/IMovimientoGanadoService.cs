using SIGA.Application.DTOs.Movimientos;

namespace SIGA.Application.Interfaces;

public interface IMovimientoGanadoService
{
    Task<IReadOnlyList<MovimientoGanadoDto>> ListarPorCaptacionAsync(Guid captacionId, CancellationToken ct = default);
    Task<MovimientoGanadoDto> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<MovimientoGanadoDto> CrearAsync(CreateMovimientoGanadoDto dto, CancellationToken ct = default);
}

using SIGA.Application.DTOs.Captaciones;

namespace SIGA.Application.Interfaces;

public interface ICaptacionGanadoService
{
    /// <summary>Si estanciaId es null, devuelve las captaciones de todas las Estancias.</summary>
    Task<IReadOnlyList<CaptacionGanadoDto>> ListarPorEstanciaAsync(Guid? estanciaId, CancellationToken ct = default);
    Task<CaptacionGanadoDto> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<CaptacionGanadoDto> CrearAsync(CreateCaptacionGanadoDto dto, CancellationToken ct = default);
    Task<CaptacionGanadoDto> ActualizarAsync(Guid id, UpdateCaptacionGanadoDto dto, CancellationToken ct = default);
    Task EliminarAsync(Guid id, CancellationToken ct = default);

    Task<DetalleLoteGanadoDto> AgregarDetalleAsync(Guid captacionId, CreateDetalleLoteGanadoDto dto, CancellationToken ct = default);
    Task EliminarDetalleAsync(Guid captacionId, Guid detalleId, CancellationToken ct = default);
}

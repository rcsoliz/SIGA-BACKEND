using SIGA.Application.DTOs.Pesaje;

namespace SIGA.Application.Interfaces;

public interface IRegistroPesajeService
{
    Task<IReadOnlyList<RegistroPesajeDto>> ListarPorCaptacionAsync(Guid captacionId, CancellationToken ct = default);
    Task<RegistroPesajeDto> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<RegistroPesajeDto> CrearAsync(CreateRegistroPesajeDto dto, CancellationToken ct = default);
}

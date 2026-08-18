using SIGA.Application.DTOs.Alimentacion;

namespace SIGA.Application.Interfaces;

public interface IRegistroAlimentacionService
{
    Task<IReadOnlyList<RegistroAlimentacionDto>> ListarPorCaptacionAsync(Guid captacionId, CancellationToken ct = default);
    Task<RegistroAlimentacionDto> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<RegistroAlimentacionDto> CrearAsync(CreateRegistroAlimentacionDto dto, CancellationToken ct = default);
}

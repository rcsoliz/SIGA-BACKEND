using SIGA.Application.DTOs.Sanitario;

namespace SIGA.Application.Interfaces;

public interface IRegistroSanitarioService
{
    Task<IReadOnlyList<RegistroSanitarioDto>> ListarPorCaptacionAsync(Guid captacionId, CancellationToken ct = default);
    Task<RegistroSanitarioDto> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<RegistroSanitarioDto> CrearAsync(CreateRegistroSanitarioDto dto, CancellationToken ct = default);
}

using SIGA.Application.DTOs.Registros;

namespace SIGA.Application.Interfaces;

/// <summary>
/// Vista de solo lectura que unifica Movimiento, Alimentación y Sanitario para la
/// pantalla "Maestro de Registros".
/// </summary>
public interface IRegistroCampoService
{
    Task<IReadOnlyList<RegistroCampoDto>> BuscarAsync(BuscarRegistrosQuery query, CancellationToken ct = default);
}

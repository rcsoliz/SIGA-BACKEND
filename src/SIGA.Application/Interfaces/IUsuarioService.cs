using SIGA.Application.DTOs.Usuarios;

namespace SIGA.Application.Interfaces;

public interface IUsuarioService
{
    Task<IReadOnlyList<UsuarioDto>> ListarAsync(CancellationToken ct = default);
    Task<UsuarioDetalleDto> ObtenerDetalleAsync(Guid id, CancellationToken ct = default);
    Task<UsuarioDto> CrearAsync(CreateUsuarioDto dto, CancellationToken ct = default);
    Task<UsuarioDto> ActualizarAsync(Guid id, UpdateUsuarioDto dto, CancellationToken ct = default);
    Task<SectorAsignadoDto> AsignarSectorAsync(Guid usuarioId, CreateSectorAsignadoDto dto, CancellationToken ct = default);
    Task QuitarSectorAsync(Guid usuarioId, Guid sectorId, CancellationToken ct = default);
    Task RevocarDispositivoAsync(Guid usuarioId, Guid dispositivoId, CancellationToken ct = default);
    Task<PermisoUsuarioDto> AsignarPermisoAsync(Guid usuarioId, AsignarPermisoDto dto, CancellationToken ct = default);
}

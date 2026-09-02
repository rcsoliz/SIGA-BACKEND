using SIGA.Domain.Entities;

namespace SIGA.Application.Interfaces;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
    Task<Usuario?> GetConDetalleAsync(Guid id, CancellationToken ct = default);
    Task AgregarSectorAsync(SectorAsignado sector, CancellationToken ct = default);
    Task AgregarPermisoAsync(PermisoUsuario permiso, CancellationToken ct = default);
}

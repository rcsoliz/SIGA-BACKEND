namespace SIGA.Application.DTOs.Usuarios;

public record UsuarioDetalleDto(
    Guid Id,
    string Nombre,
    string Email,
    string? Cargo,
    string Rol,
    string Estado,
    DateTime FechaCreacion,
    IReadOnlyList<SectorAsignadoDto> SectoresAsignados,
    IReadOnlyList<DispositivoDto> Dispositivos,
    IReadOnlyList<PermisoUsuarioDto> Permisos);

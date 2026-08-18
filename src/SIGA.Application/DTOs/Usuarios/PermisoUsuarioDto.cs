namespace SIGA.Application.DTOs.Usuarios;

public record PermisoUsuarioDto(Guid Id, string TipoPermiso);

public record AsignarPermisoDto(string TipoPermiso);

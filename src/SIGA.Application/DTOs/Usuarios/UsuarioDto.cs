namespace SIGA.Application.DTOs.Usuarios;

public record UsuarioDto(
    Guid Id,
    string Nombre,
    string Email,
    string? Cargo,
    string Rol,
    string Estado,
    DateTime FechaCreacion);

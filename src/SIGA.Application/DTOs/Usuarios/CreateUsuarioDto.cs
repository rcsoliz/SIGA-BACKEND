namespace SIGA.Application.DTOs.Usuarios;

public record CreateUsuarioDto(
    string Nombre,
    string Email,
    string Password,
    string? Cargo,
    string Rol);

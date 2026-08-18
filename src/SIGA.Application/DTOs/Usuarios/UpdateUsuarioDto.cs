namespace SIGA.Application.DTOs.Usuarios;

public record UpdateUsuarioDto(
    string Nombre,
    string? Cargo,
    string Estado);

namespace SIGA.Application.DTOs.Auth;

public record LoginResponseDto(
    string Token,
    DateTime ExpiraEnUtc,
    Guid UsuarioId,
    string Nombre,
    string Rol);

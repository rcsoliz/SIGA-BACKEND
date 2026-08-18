using SIGA.Application.Common.Exceptions;
using SIGA.Application.DTOs.Auth;
using SIGA.Application.Interfaces;
using SIGA.Domain.Enums;

namespace SIGA.Application.Services;

public class AuthService(
    IUsuarioRepository usuarioRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator) : IAuthService
{
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct = default)
    {
        var usuario = await usuarioRepository.GetByEmailAsync(request.Email, ct)
            ?? throw new UnauthorizedException("Credenciales inválidas.");

        if (!passwordHasher.Verify(request.Password, usuario.PasswordHash))
        {
            throw new UnauthorizedException("Credenciales inválidas.");
        }

        if (usuario.Estado != EstadoUsuario.Activo)
        {
            throw new UnauthorizedException($"El usuario se encuentra en estado '{usuario.Estado}'.");
        }

        var (token, expiraEnUtc) = jwtTokenGenerator.Generar(usuario);

        return new LoginResponseDto(token, expiraEnUtc, usuario.Id, usuario.Nombre, usuario.Rol.ToString());
    }
}

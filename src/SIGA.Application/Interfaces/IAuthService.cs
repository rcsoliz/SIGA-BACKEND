using SIGA.Application.DTOs.Auth;

namespace SIGA.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct = default);
}

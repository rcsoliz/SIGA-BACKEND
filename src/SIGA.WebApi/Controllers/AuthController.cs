using Microsoft.AspNetCore.Mvc;
using SIGA.Application.DTOs.Auth;
using SIGA.Application.Interfaces;

namespace SIGA.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request, CancellationToken ct)
    {
        var respuesta = await authService.LoginAsync(request, ct);
        return Ok(respuesta);
    }
}

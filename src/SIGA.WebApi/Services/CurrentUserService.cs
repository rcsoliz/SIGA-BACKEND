using System.Security.Claims;
using SIGA.Application.Interfaces;

namespace SIGA.WebApi.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? UsuarioId
    {
        get
        {
            var valor = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(valor, out var id) ? id : null;
        }
    }

    public string? Rol => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
}

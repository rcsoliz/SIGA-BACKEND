namespace SIGA.Application.Interfaces;

/// <summary>
/// Expone al usuario autenticado de la petición HTTP actual. Se resuelve leyendo los
/// claims del JWT; la implementación vive en WebApi porque depende de HttpContext.
/// </summary>
public interface ICurrentUserService
{
    Guid? UsuarioId { get; }
    string? Rol { get; }
}

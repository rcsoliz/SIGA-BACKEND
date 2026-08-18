using SIGA.Domain.Common;
using SIGA.Domain.Enums;

namespace SIGA.Domain.Entities;

/// <summary>
/// Base de la jerarquía de autenticación (Usuario &lt;|-- Captador, Usuario &lt;|-- Administrador).
/// El rol queda fijado por el tipo concreto; se expone también como enum para claims JWT
/// y consultas sin necesidad de comprobar el tipo en tiempo de ejecución.
/// </summary>
public abstract class Usuario : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Cargo { get; set; }
    public RolUsuario Rol { get; protected set; }
    public EstadoUsuario Estado { get; set; } = EstadoUsuario.Pendiente;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public ICollection<SectorAsignado> SectoresAsignados { get; set; } = new List<SectorAsignado>();
    public ICollection<Dispositivo> Dispositivos { get; set; } = new List<Dispositivo>();
    public ICollection<PermisoUsuario> Permisos { get; set; } = new List<PermisoUsuario>();

    protected Usuario(RolUsuario rol)
    {
        Rol = rol;
    }
}

using SIGA.Domain.Common;
using SIGA.Domain.Enums;

namespace SIGA.Domain.Entities;

/// <summary>
/// Bitácora transversal de acciones (creación/modificación/eliminación) sobre cualquier
/// módulo. Se escribe automáticamente desde la capa de aplicación, nunca por el usuario.
/// </summary>
public class LogAuditoria : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public AccionAuditoria Accion { get; set; }
    public string Modulo { get; set; } = string.Empty;
    public Guid IdRegistroAfectado { get; set; }
    public DateTime FechaHora { get; set; } = DateTime.UtcNow;
    public string? Detalle { get; set; }
}

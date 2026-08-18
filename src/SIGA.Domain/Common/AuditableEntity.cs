using SIGA.Domain.Enums;

namespace SIGA.Domain.Common;

/// <summary>
/// Campos de trazabilidad requeridos por la captura offline: el dispositivo genera el
/// registro sin conexión y estos campos permiten reconstruir quién, cuándo y si ya
/// llegó al servidor.
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public Guid CreadoPorUsuarioId { get; set; }
    public DateTime FechaCreacionLocal { get; set; }
    public DateTime? FechaSincronizacion { get; set; }
    public EstadoSync EstadoSync { get; set; } = EstadoSync.Pendiente;
    public Guid? ModificadoPorUsuarioId { get; set; }
    public DateTime? FechaModificacion { get; set; }
}

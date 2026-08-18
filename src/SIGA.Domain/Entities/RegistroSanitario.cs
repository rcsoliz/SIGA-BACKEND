using SIGA.Domain.Common;
using SIGA.Domain.Enums;

namespace SIGA.Domain.Entities;

public class RegistroSanitario : AuditableEntity
{
    public Guid CaptacionGanadoId { get; set; }
    public CaptacionGanado CaptacionGanado { get; set; } = null!;

    public DateTime Fecha { get; set; }
    public TipoEventoSanitario TipoEvento { get; set; }
    public string? ProductoTratamiento { get; set; }
    public Guid RegistradoPorUsuarioId { get; set; }
    public Usuario RegistradoPor { get; set; } = null!;
    public string? Observaciones { get; set; }
}

using SIGA.Domain.Common;
using SIGA.Domain.Enums;

namespace SIGA.Domain.Entities;

/// <summary>
/// Traslado físico de una captación entre potreros/corrales. Origen y destino son
/// ubicaciones (no captaciones distintas): la captación afectada sigue siendo la misma,
/// solo cambia su localización — que además actualiza CaptacionGanado.Potrero al
/// confirmarse el movimiento.
/// </summary>
public class MovimientoGanado : AuditableEntity
{
    public Guid CaptacionGanadoId { get; set; }
    public CaptacionGanado CaptacionGanado { get; set; } = null!;

    public DateTime Fecha { get; set; }
    public CategoriaGanado TipoGanado { get; set; }
    public int CantidadCabezas { get; set; }
    public string Origen { get; set; } = string.Empty;
    public string Destino { get; set; } = string.Empty;
}

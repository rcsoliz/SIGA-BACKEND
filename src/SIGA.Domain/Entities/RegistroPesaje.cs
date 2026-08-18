using SIGA.Domain.Common;

namespace SIGA.Domain.Entities;

/// <summary>
/// Control de peso periódico de una captación (ej. "Control de peso mensual"), visto en
/// el historial de eventos de Reporte de Lote junto a Movimiento/Sanidad/Formación.
/// </summary>
public class RegistroPesaje : AuditableEntity
{
    public Guid CaptacionGanadoId { get; set; }
    public CaptacionGanado CaptacionGanado { get; set; } = null!;

    public DateTime Fecha { get; set; }
    public double PesoPromedioKg { get; set; }
    public int? CantidadCabezasPesadas { get; set; }
    public string? Observaciones { get; set; }
}

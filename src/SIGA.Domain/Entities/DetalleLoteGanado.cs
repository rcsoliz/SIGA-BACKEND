using SIGA.Domain.Common;
using SIGA.Domain.Enums;

namespace SIGA.Domain.Entities;

/// <summary>
/// Detalle: una fila por cada categoría de animal dentro de una CaptacionGanado. El
/// estado de sincronización vive en la cabecera (CaptacionGanado) — todo el detalle se
/// crea y sincroniza junto con ella en una sola sesión de captura offline.
/// </summary>
public class DetalleLoteGanado : BaseEntity
{
    public Guid CaptacionGanadoId { get; set; }
    public CaptacionGanado CaptacionGanado { get; set; } = null!;

    public CategoriaGanado Categoria { get; set; }
    public string? Raza { get; set; }
    public int CantidadCabezas { get; set; }
    public double? PesoPromedioEstimadoKg { get; set; }
    public TipoManejoAlimentario SistemaAlimentacion { get; set; }
    public DateTime? FechaEstimadaFaena { get; set; }
    public string? NotasZootecnicas { get; set; }

    public Guid CreadoPor { get; set; }
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    public Guid? ActualizadoPor { get; set; }
    public DateTime? ActualizadoEn { get; set; }

    public double CalcularPesoLote() => CantidadCabezas * (PesoPromedioEstimadoKg ?? 0);

    public int? GetDiasRestantesFaena() =>
        FechaEstimadaFaena is { } fecha ? (int)(fecha.Date - DateTime.UtcNow.Date).TotalDays : null;

    public bool EsAptoParaFaenaInmediata() => GetDiasRestantesFaena() is <= 0;
}

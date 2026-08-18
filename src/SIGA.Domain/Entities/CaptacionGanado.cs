using SIGA.Domain.Common;
using SIGA.Domain.Enums;

namespace SIGA.Domain.Entities;

/// <summary>
/// Cabecera: un evento de captura/visita de campo. Puede agrupar varias categorías de
/// animales distintas (ver DetalleLoteGanado) encontradas en la misma visita a la
/// Estancia — ej. 20 vacas + 5 toros + 10 vaquillas en una sola captación.
/// </summary>
public class CaptacionGanado : AuditableEntity
{
    public Guid EstanciaId { get; set; }
    public Estancia Estancia { get; set; } = null!;

    /// <summary>Identificador legible para UI (dropdowns, reportes) — ej. "Lote Norte A".</summary>
    public string Nombre { get; set; } = string.Empty;

    public string? Observaciones { get; set; }
    public EstadoCaptacion Estado { get; set; } = EstadoCaptacion.BorradorLocal;
    public EstadoSanitario EstadoSanitario { get; set; } = EstadoSanitario.Optimo;
    public string? Potrero { get; set; }
    public DateTime Fecha { get; set; }

    /// <summary>
    /// Punto GPS capturado en esta visita (puede diferir del de Estancia.Latitud/Longitud
    /// si el captador está en un potrero distinto al punto de referencia del predio).
    /// Permite ubicar cada captación por separado en Mapa de Operaciones / Mapa de Lotes.
    /// </summary>
    public double? Latitud { get; set; }
    public double? Longitud { get; set; }

    public ICollection<DetalleLoteGanado> Detalles { get; set; } = new List<DetalleLoteGanado>();
    public ICollection<RegistroAlimentacion> RegistrosAlimentacion { get; set; } = new List<RegistroAlimentacion>();
    public ICollection<RegistroSanitario> RegistrosSanitarios { get; set; } = new List<RegistroSanitario>();
    public ICollection<MovimientoGanado> Movimientos { get; set; } = new List<MovimientoGanado>();

    public void AgregarDetalleLote(DetalleLoteGanado detalle)
    {
        detalle.CaptacionGanadoId = Id;
        Detalles.Add(detalle);
    }

    public bool EliminarDetalleLote(Guid idDetalle)
    {
        var detalle = Detalles.FirstOrDefault(d => d.Id == idDetalle);
        return detalle is not null && Detalles.Remove(detalle);
    }

    public int CalcularTotalCabezas() => Detalles.Sum(d => d.CantidadCabezas);

    public double CalcularPesoEstimadoTotal() => Detalles.Sum(d => d.CalcularPesoLote());
}

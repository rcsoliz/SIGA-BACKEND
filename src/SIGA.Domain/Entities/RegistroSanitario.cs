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

    /// <summary>Catálogo normalizado, opcional — la columna ProductoTratamiento de arriba se mantiene como legado.</summary>
    public Guid? ProductoTratamientoId { get; set; }
    public ProductoTratamiento? ProductoTratamientoCatalogo { get; set; }

    public Guid RegistradoPorUsuarioId { get; set; }
    public Usuario RegistradoPor { get; set; } = null!;
    public string? Observaciones { get; set; }
}

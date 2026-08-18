using SIGA.Domain.Common;
using SIGA.Domain.Enums;

namespace SIGA.Domain.Entities;

public class RegistroAlimentacion : AuditableEntity
{
    public Guid CaptacionGanadoId { get; set; }
    public CaptacionGanado CaptacionGanado { get; set; } = null!;

    public DateTime Fecha { get; set; }
    public TipoManejoAlimentario TipoAlimentacion { get; set; }
    public double? RacionBaseKgAnimal { get; set; }
    public double? SuplementoProteicoKgAnimal { get; set; }
    public string? Observaciones { get; set; }
}

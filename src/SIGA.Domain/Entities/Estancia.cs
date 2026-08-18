using SIGA.Domain.Common;

namespace SIGA.Domain.Entities;

public class Estancia : AuditableEntity
{
    public Guid CaptadorId { get; set; }
    public Captador Captador { get; set; } = null!;

    public string Nombre { get; set; } = string.Empty;
    public string Propietario { get; set; } = string.Empty;
    public string? Representante { get; set; }
    public string? Telefono { get; set; }

    public double Latitud { get; set; }
    public double Longitud { get; set; }

    public string? Renspa { get; set; }
    public double? HectareasTotales { get; set; }
    public string? Departamento { get; set; }
    public string? Provincia { get; set; }
    public string? Municipio { get; set; }

    public ICollection<CaptacionGanado> Captaciones { get; set; } = new List<CaptacionGanado>();
}

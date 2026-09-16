using SIGA.Domain.Common;

namespace SIGA.Domain.Entities;

public class Provincia : BaseEntity
{
    public Guid DepartamentoId { get; set; }
    public Departamento Departamento { get; set; } = null!;

    public string Nombre { get; set; } = string.Empty;

    public ICollection<Municipio> Municipios { get; set; } = new List<Municipio>();
}

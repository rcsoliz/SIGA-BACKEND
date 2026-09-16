using SIGA.Domain.Common;

namespace SIGA.Domain.Entities;

public class Departamento : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Provincia> Provincias { get; set; } = new List<Provincia>();
}

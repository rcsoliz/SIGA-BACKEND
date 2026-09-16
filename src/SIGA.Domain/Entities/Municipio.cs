using SIGA.Domain.Common;

namespace SIGA.Domain.Entities;

public class Municipio : BaseEntity
{
    public Guid ProvinciaId { get; set; }
    public Provincia Provincia { get; set; } = null!;

    public string Nombre { get; set; } = string.Empty;
}

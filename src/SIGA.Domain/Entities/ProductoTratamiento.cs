using SIGA.Domain.Common;

namespace SIGA.Domain.Entities;

public class ProductoTratamiento : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
}

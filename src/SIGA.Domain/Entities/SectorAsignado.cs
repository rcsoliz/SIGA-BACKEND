using SIGA.Domain.Common;

namespace SIGA.Domain.Entities;

public class SectorAsignado : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public string NombreSector { get; set; } = string.Empty;
    public string? Zona { get; set; }
}

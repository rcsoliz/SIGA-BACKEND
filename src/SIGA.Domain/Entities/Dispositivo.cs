using SIGA.Domain.Common;
using SIGA.Domain.Enums;

namespace SIGA.Domain.Entities;

public class Dispositivo : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public string IdentificadorDispositivo { get; set; } = string.Empty;
    public DateTime? UltimaSincronizacion { get; set; }
    public string? UbicacionActual { get; set; }
    public EstadoDispositivo Estado { get; set; } = EstadoDispositivo.Activo;
}

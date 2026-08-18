using SIGA.Domain.Enums;

namespace SIGA.Domain.Entities;

public class Captador : Usuario
{
    public Captador() : base(RolUsuario.Captador)
    {
    }

    public ICollection<Estancia> Estancias { get; set; } = new List<Estancia>();
}

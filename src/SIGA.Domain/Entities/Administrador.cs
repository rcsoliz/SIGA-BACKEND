using SIGA.Domain.Enums;

namespace SIGA.Domain.Entities;

public class Administrador : Usuario
{
    public Administrador() : base(RolUsuario.Administrador)
    {
    }
}

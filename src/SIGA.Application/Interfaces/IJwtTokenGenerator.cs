using SIGA.Domain.Entities;

namespace SIGA.Application.Interfaces;

public interface IJwtTokenGenerator
{
    /// <returns>Token firmado y su fecha de expiración UTC.</returns>
    (string Token, DateTime ExpiraEnUtc) Generar(Usuario usuario);
}

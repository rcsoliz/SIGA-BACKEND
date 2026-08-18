using SIGA.Application.Common;
using SIGA.Application.Common.Exceptions;
using SIGA.Application.DTOs.Alimentacion;
using SIGA.Application.Interfaces;
using SIGA.Domain.Entities;
using SIGA.Domain.Enums;

namespace SIGA.Application.Services;

public class RegistroAlimentacionService(
    IRegistroAlimentacionRepository repository,
    ICaptacionGanadoRepository captacionRepository,
    ICurrentUserService currentUserService,
    IAuditoriaService auditoriaService) : IRegistroAlimentacionService
{
    public async Task<IReadOnlyList<RegistroAlimentacionDto>> ListarPorCaptacionAsync(Guid captacionId, CancellationToken ct = default)
    {
        var registros = await repository.GetByCaptacionAsync(captacionId, ct);
        return registros.Select(ToDto).ToList();
    }

    public async Task<RegistroAlimentacionDto> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var registro = await repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(RegistroAlimentacion), id);

        return ToDto(registro);
    }

    public async Task<RegistroAlimentacionDto> CrearAsync(CreateRegistroAlimentacionDto dto, CancellationToken ct = default)
    {
        _ = await captacionRepository.GetByIdAsync(dto.CaptacionGanadoId, ct)
            ?? throw new NotFoundException(nameof(CaptacionGanado), dto.CaptacionGanadoId);

        var usuarioId = currentUserService.UsuarioId
            ?? throw new UnauthorizedException("Se requiere un usuario autenticado.");

        var registro = new RegistroAlimentacion
        {
            CaptacionGanadoId = dto.CaptacionGanadoId,
            Fecha = dto.Fecha,
            TipoAlimentacion = EnumParser.Parse<TipoManejoAlimentario>(dto.TipoAlimentacion, nameof(dto.TipoAlimentacion)),
            RacionBaseKgAnimal = dto.RacionBaseKgAnimal,
            SuplementoProteicoKgAnimal = dto.SuplementoProteicoKgAnimal,
            Observaciones = dto.Observaciones,
            CreadoPorUsuarioId = usuarioId,
            FechaCreacionLocal = dto.FechaCreacionLocal,
            EstadoSync = EstadoSync.Sincronizado
        };

        await repository.AddAsync(registro, ct);
        await repository.SaveChangesAsync(ct);
        await auditoriaService.RegistrarAsync(AccionAuditoria.Creacion, "Alimentacion", registro.Id, ct: ct);

        return ToDto(registro);
    }

    private static RegistroAlimentacionDto ToDto(RegistroAlimentacion r) => new(
        r.Id,
        r.CaptacionGanadoId,
        r.Fecha,
        r.TipoAlimentacion.ToString(),
        r.RacionBaseKgAnimal,
        r.SuplementoProteicoKgAnimal,
        r.Observaciones,
        r.EstadoSync.ToString());
}

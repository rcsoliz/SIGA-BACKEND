using SIGA.Application.Common.Exceptions;
using SIGA.Application.DTOs.Pesaje;
using SIGA.Application.Interfaces;
using SIGA.Domain.Entities;
using SIGA.Domain.Enums;

namespace SIGA.Application.Services;

public class RegistroPesajeService(
    IRegistroPesajeRepository repository,
    ICaptacionGanadoRepository captacionRepository,
    ICurrentUserService currentUserService,
    IAuditoriaService auditoriaService) : IRegistroPesajeService
{
    public async Task<IReadOnlyList<RegistroPesajeDto>> ListarPorCaptacionAsync(Guid captacionId, CancellationToken ct = default)
    {
        var registros = await repository.GetByCaptacionAsync(captacionId, ct);
        return registros.Select(ToDto).ToList();
    }

    public async Task<RegistroPesajeDto> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var registro = await repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(RegistroPesaje), id);

        return ToDto(registro);
    }

    public async Task<RegistroPesajeDto> CrearAsync(CreateRegistroPesajeDto dto, CancellationToken ct = default)
    {
        _ = await captacionRepository.GetByIdAsync(dto.CaptacionGanadoId, ct)
            ?? throw new NotFoundException(nameof(CaptacionGanado), dto.CaptacionGanadoId);

        var usuarioId = currentUserService.UsuarioId
            ?? throw new UnauthorizedException("Se requiere un usuario autenticado.");

        var registro = new RegistroPesaje
        {
            CaptacionGanadoId = dto.CaptacionGanadoId,
            Fecha = dto.Fecha,
            PesoPromedioKg = dto.PesoPromedioKg,
            CantidadCabezasPesadas = dto.CantidadCabezasPesadas,
            Observaciones = dto.Observaciones,
            CreadoPorUsuarioId = usuarioId,
            FechaCreacionLocal = dto.FechaCreacionLocal,
            EstadoSync = EstadoSync.Sincronizado
        };

        await repository.AddAsync(registro, ct);
        await repository.SaveChangesAsync(ct);
        await auditoriaService.RegistrarAsync(AccionAuditoria.Creacion, "Pesaje", registro.Id, $"{dto.PesoPromedioKg} kg", ct);

        return ToDto(registro);
    }

    private static RegistroPesajeDto ToDto(RegistroPesaje r) => new(
        r.Id,
        r.CaptacionGanadoId,
        r.Fecha,
        r.PesoPromedioKg,
        r.CantidadCabezasPesadas,
        r.Observaciones,
        r.EstadoSync.ToString());
}

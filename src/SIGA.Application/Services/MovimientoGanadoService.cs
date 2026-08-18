using SIGA.Application.Common;
using SIGA.Application.Common.Exceptions;
using SIGA.Application.DTOs.Movimientos;
using SIGA.Application.Interfaces;
using SIGA.Domain.Entities;
using SIGA.Domain.Enums;

namespace SIGA.Application.Services;

public class MovimientoGanadoService(
    IMovimientoGanadoRepository repository,
    ICaptacionGanadoRepository captacionRepository,
    ICurrentUserService currentUserService,
    IAuditoriaService auditoriaService) : IMovimientoGanadoService
{
    public async Task<IReadOnlyList<MovimientoGanadoDto>> ListarPorCaptacionAsync(Guid captacionId, CancellationToken ct = default)
    {
        var movimientos = await repository.GetByCaptacionAsync(captacionId, ct);
        return movimientos.Select(ToDto).ToList();
    }

    public async Task<MovimientoGanadoDto> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var movimiento = await repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(MovimientoGanado), id);

        return ToDto(movimiento);
    }

    public async Task<MovimientoGanadoDto> CrearAsync(CreateMovimientoGanadoDto dto, CancellationToken ct = default)
    {
        var captacion = await captacionRepository.GetByIdAsync(dto.CaptacionGanadoId, ct)
            ?? throw new NotFoundException(nameof(CaptacionGanado), dto.CaptacionGanadoId);

        var usuarioId = currentUserService.UsuarioId
            ?? throw new UnauthorizedException("Se requiere un usuario autenticado.");

        var movimiento = new MovimientoGanado
        {
            CaptacionGanadoId = dto.CaptacionGanadoId,
            Fecha = dto.Fecha,
            TipoGanado = EnumParser.Parse<CategoriaGanado>(dto.TipoGanado, nameof(dto.TipoGanado)),
            CantidadCabezas = dto.CantidadCabezas,
            Origen = dto.Origen,
            Destino = dto.Destino,
            CreadoPorUsuarioId = usuarioId,
            FechaCreacionLocal = dto.FechaCreacionLocal,
            EstadoSync = EstadoSync.Sincronizado
        };

        await repository.AddAsync(movimiento, ct);

        // El traslado confirmado actualiza la ubicación vigente de la captación.
        captacion.Potrero = dto.Destino;
        captacionRepository.Update(captacion);

        await repository.SaveChangesAsync(ct);
        await auditoriaService.RegistrarAsync(AccionAuditoria.Creacion, "Movimiento", movimiento.Id, $"{dto.Origen} -> {dto.Destino}", ct);

        return ToDto(movimiento);
    }

    private static MovimientoGanadoDto ToDto(MovimientoGanado m) => new(
        m.Id,
        m.CaptacionGanadoId,
        m.Fecha,
        m.TipoGanado.ToString(),
        m.CantidadCabezas,
        m.Origen,
        m.Destino,
        m.EstadoSync.ToString());
}

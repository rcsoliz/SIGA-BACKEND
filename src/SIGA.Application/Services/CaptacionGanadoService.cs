using SIGA.Application.Common;
using SIGA.Application.Common.Exceptions;
using SIGA.Application.DTOs.Captaciones;
using SIGA.Application.Interfaces;
using SIGA.Domain.Entities;
using SIGA.Domain.Enums;

namespace SIGA.Application.Services;

public class CaptacionGanadoService(
    ICaptacionGanadoRepository captacionRepository,
    IEstanciaRepository estanciaRepository,
    IMovimientoGanadoRepository movimientoRepository,
    ICurrentUserService currentUserService,
    IAuditoriaService auditoriaService) : ICaptacionGanadoService
{
    public async Task<IReadOnlyList<CaptacionGanadoDto>> ListarPorEstanciaAsync(Guid? estanciaId, CancellationToken ct = default)
    {
        var captaciones = estanciaId is null
            ? await captacionRepository.GetAllAsync(ct)
            : await captacionRepository.GetByEstanciaAsync(estanciaId.Value, ct);
        var dtos = new List<CaptacionGanadoDto>();
        foreach (var captacion in captaciones)
        {
            dtos.Add(await ToDtoAsync(captacion, ct));
        }
        return dtos;
    }

    public async Task<CaptacionGanadoDto> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var captacion = await captacionRepository.GetConDetallesAsync(id, ct)
            ?? throw new NotFoundException(nameof(CaptacionGanado), id);

        return await ToDtoAsync(captacion, ct);
    }

    public async Task<CaptacionGanadoDto> CrearAsync(CreateCaptacionGanadoDto dto, CancellationToken ct = default)
    {
        if (dto.Detalles.Count == 0)
        {
            throw new ValidationException("Una captación debe incluir al menos un detalle de lote.");
        }

        _ = await estanciaRepository.GetByIdAsync(dto.EstanciaId, ct)
            ?? throw new NotFoundException(nameof(Estancia), dto.EstanciaId);

        var usuarioId = currentUserService.UsuarioId
            ?? throw new UnauthorizedException("Se requiere un usuario autenticado.");

        var captacion = new CaptacionGanado
        {
            EstanciaId = dto.EstanciaId,
            Nombre = dto.Nombre,
            Observaciones = dto.Observaciones,
            Potrero = dto.Potrero,
            Fecha = dto.Fecha,
            Latitud = dto.Latitud,
            Longitud = dto.Longitud,
            Estado = EstadoCaptacion.Registrado,
            CreadoPorUsuarioId = usuarioId,
            FechaCreacionLocal = dto.FechaCreacionLocal,
            EstadoSync = EstadoSync.Sincronizado
        };

        foreach (var detalleDto in dto.Detalles)
        {
            captacion.AgregarDetalleLote(new DetalleLoteGanado
            {
                Categoria = EnumParser.Parse<CategoriaGanado>(detalleDto.Categoria, nameof(detalleDto.Categoria)),
                Raza = detalleDto.Raza,
                CantidadCabezas = detalleDto.CantidadCabezas,
                PesoPromedioEstimadoKg = detalleDto.PesoPromedioEstimadoKg,
                SistemaAlimentacion = EnumParser.Parse<TipoManejoAlimentario>(detalleDto.SistemaAlimentacion, nameof(detalleDto.SistemaAlimentacion)),
                FechaEstimadaFaena = detalleDto.FechaEstimadaFaena,
                NotasZootecnicas = detalleDto.NotasZootecnicas,
                CreadoPor = usuarioId,
                CreadoEn = DateTime.UtcNow
            });
        }

        await captacionRepository.AddAsync(captacion, ct);
        await captacionRepository.SaveChangesAsync(ct);
        await auditoriaService.RegistrarAsync(AccionAuditoria.Creacion, "CaptacionGanado", captacion.Id, captacion.Nombre, ct);

        return await ToDtoAsync(captacion, ct);
    }

    public async Task<CaptacionGanadoDto> ActualizarAsync(Guid id, UpdateCaptacionGanadoDto dto, CancellationToken ct = default)
    {
        var captacion = await captacionRepository.GetConDetallesAsync(id, ct)
            ?? throw new NotFoundException(nameof(CaptacionGanado), id);

        captacion.Nombre = dto.Nombre;
        captacion.Observaciones = dto.Observaciones;
        captacion.Estado = EnumParser.Parse<EstadoCaptacion>(dto.Estado, nameof(dto.Estado));
        captacion.EstadoSanitario = EnumParser.Parse<EstadoSanitario>(dto.EstadoSanitario, nameof(dto.EstadoSanitario));
        captacion.Potrero = dto.Potrero;
        captacion.ModificadoPorUsuarioId = currentUserService.UsuarioId;
        captacion.FechaModificacion = DateTime.UtcNow;

        captacionRepository.Update(captacion);
        await captacionRepository.SaveChangesAsync(ct);
        await auditoriaService.RegistrarAsync(AccionAuditoria.Modificacion, "CaptacionGanado", captacion.Id, captacion.Nombre, ct);

        return await ToDtoAsync(captacion, ct);
    }

    public async Task EliminarAsync(Guid id, CancellationToken ct = default)
    {
        var captacion = await captacionRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(CaptacionGanado), id);

        captacionRepository.Remove(captacion);
        await captacionRepository.SaveChangesAsync(ct);
        await auditoriaService.RegistrarAsync(AccionAuditoria.Eliminacion, "CaptacionGanado", id, captacion.Nombre, ct);
    }

    public async Task<DetalleLoteGanadoDto> AgregarDetalleAsync(Guid captacionId, CreateDetalleLoteGanadoDto dto, CancellationToken ct = default)
    {
        var captacion = await captacionRepository.GetConDetallesAsync(captacionId, ct)
            ?? throw new NotFoundException(nameof(CaptacionGanado), captacionId);

        var usuarioId = currentUserService.UsuarioId
            ?? throw new UnauthorizedException("Se requiere un usuario autenticado.");

        var detalle = new DetalleLoteGanado
        {
            Categoria = EnumParser.Parse<CategoriaGanado>(dto.Categoria, nameof(dto.Categoria)),
            Raza = dto.Raza,
            CantidadCabezas = dto.CantidadCabezas,
            PesoPromedioEstimadoKg = dto.PesoPromedioEstimadoKg,
            SistemaAlimentacion = EnumParser.Parse<TipoManejoAlimentario>(dto.SistemaAlimentacion, nameof(dto.SistemaAlimentacion)),
            FechaEstimadaFaena = dto.FechaEstimadaFaena,
            NotasZootecnicas = dto.NotasZootecnicas,
            CreadoPor = usuarioId,
            CreadoEn = DateTime.UtcNow
        };

        captacion.AgregarDetalleLote(detalle);
        captacionRepository.Update(captacion);
        await captacionRepository.SaveChangesAsync(ct);
        await auditoriaService.RegistrarAsync(AccionAuditoria.Modificacion, "CaptacionGanado.Detalles", captacionId, $"+{detalle.Categoria}: {detalle.CantidadCabezas}", ct);

        return ToDto(detalle);
    }

    public async Task EliminarDetalleAsync(Guid captacionId, Guid detalleId, CancellationToken ct = default)
    {
        var captacion = await captacionRepository.GetConDetallesAsync(captacionId, ct)
            ?? throw new NotFoundException(nameof(CaptacionGanado), captacionId);

        if (!captacion.EliminarDetalleLote(detalleId))
        {
            throw new NotFoundException(nameof(DetalleLoteGanado), detalleId);
        }

        captacionRepository.Update(captacion);
        await captacionRepository.SaveChangesAsync(ct);
        await auditoriaService.RegistrarAsync(AccionAuditoria.Modificacion, "CaptacionGanado.Detalles", captacionId, $"-{detalleId}", ct);
    }

    private async Task<CaptacionGanadoDto> ToDtoAsync(CaptacionGanado c, CancellationToken ct)
    {
        var diasEnPotrero = await CalcularDiasEnPotreroAsync(c, ct);

        return new CaptacionGanadoDto(
            c.Id,
            c.EstanciaId,
            c.Nombre,
            c.Observaciones,
            c.Estado.ToString(),
            c.EstadoSanitario.ToString(),
            c.Potrero,
            c.Fecha,
            c.Latitud,
            c.Longitud,
            c.EstadoSync.ToString(),
            c.CalcularTotalCabezas(),
            c.CalcularPesoEstimadoTotal(),
            diasEnPotrero,
            c.Detalles.Select(ToDto).ToList());
    }

    /// <summary>
    /// Días desde el último traslado confirmado (o desde la fecha de la captación si
    /// todavía no hay movimientos), usado para "Días en Potrero" en Reporte de Lote.
    /// </summary>
    private async Task<int> CalcularDiasEnPotreroAsync(CaptacionGanado c, CancellationToken ct)
    {
        var movimientos = await movimientoRepository.GetByCaptacionAsync(c.Id, ct);
        var fechaReferencia = movimientos.Count > 0 ? movimientos[0].Fecha : c.Fecha;
        return Math.Max(0, (int)(DateTime.UtcNow.Date - fechaReferencia.Date).TotalDays);
    }

    private static DetalleLoteGanadoDto ToDto(DetalleLoteGanado d) => new(
        d.Id,
        d.Categoria.ToString(),
        d.Raza,
        d.CantidadCabezas,
        d.PesoPromedioEstimadoKg,
        d.SistemaAlimentacion.ToString(),
        d.FechaEstimadaFaena,
        d.NotasZootecnicas,
        d.CalcularPesoLote(),
        d.GetDiasRestantesFaena());
}

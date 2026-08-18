using SIGA.Application.Common;
using SIGA.Application.Common.Exceptions;
using SIGA.Application.DTOs.Sanitario;
using SIGA.Application.Interfaces;
using SIGA.Domain.Entities;
using SIGA.Domain.Enums;

namespace SIGA.Application.Services;

public class RegistroSanitarioService(
    IRegistroSanitarioRepository repository,
    ICaptacionGanadoRepository captacionRepository,
    IUsuarioRepository usuarioRepository,
    ICurrentUserService currentUserService,
    IAuditoriaService auditoriaService) : IRegistroSanitarioService
{
    public async Task<IReadOnlyList<RegistroSanitarioDto>> ListarPorCaptacionAsync(Guid captacionId, CancellationToken ct = default)
    {
        var registros = await repository.GetByCaptacionAsync(captacionId, ct);
        return registros.Select(ToDto).ToList();
    }

    public async Task<RegistroSanitarioDto> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var registro = await repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(RegistroSanitario), id);

        return ToDto(registro);
    }

    public async Task<RegistroSanitarioDto> CrearAsync(CreateRegistroSanitarioDto dto, CancellationToken ct = default)
    {
        _ = await captacionRepository.GetByIdAsync(dto.CaptacionGanadoId, ct)
            ?? throw new NotFoundException(nameof(CaptacionGanado), dto.CaptacionGanadoId);

        var usuarioId = currentUserService.UsuarioId
            ?? throw new UnauthorizedException("Se requiere un usuario autenticado.");
        var usuario = await usuarioRepository.GetByIdAsync(usuarioId, ct)
            ?? throw new NotFoundException(nameof(Usuario), usuarioId);

        var registro = new RegistroSanitario
        {
            CaptacionGanadoId = dto.CaptacionGanadoId,
            Fecha = dto.Fecha,
            TipoEvento = EnumParser.Parse<TipoEventoSanitario>(dto.TipoEvento, nameof(dto.TipoEvento)),
            ProductoTratamiento = dto.ProductoTratamiento,
            RegistradoPorUsuarioId = usuarioId,
            Observaciones = dto.Observaciones,
            CreadoPorUsuarioId = usuarioId,
            FechaCreacionLocal = dto.FechaCreacionLocal,
            EstadoSync = EstadoSync.Sincronizado
        };

        await repository.AddAsync(registro, ct);
        await repository.SaveChangesAsync(ct);
        await auditoriaService.RegistrarAsync(AccionAuditoria.Creacion, "Sanitario", registro.Id, registro.ProductoTratamiento, ct);

        return ToDto(registro, usuario.Nombre);
    }

    private static RegistroSanitarioDto ToDto(RegistroSanitario r) =>
        ToDto(r, r.RegistradoPor?.Nombre ?? string.Empty);

    private static RegistroSanitarioDto ToDto(RegistroSanitario r, string registradoPorNombre) => new(
        r.Id,
        r.CaptacionGanadoId,
        r.Fecha,
        r.TipoEvento.ToString(),
        r.ProductoTratamiento,
        r.RegistradoPorUsuarioId,
        registradoPorNombre,
        r.Observaciones,
        r.EstadoSync.ToString());
}

using SIGA.Application.Common.Exceptions;
using SIGA.Application.DTOs.Estancias;
using SIGA.Application.Interfaces;
using SIGA.Domain.Entities;
using SIGA.Domain.Enums;

namespace SIGA.Application.Services;

public class EstanciaService(
    IEstanciaRepository estanciaRepository,
    ICurrentUserService currentUserService,
    IAuditoriaService auditoriaService) : IEstanciaService
{
    public async Task<IReadOnlyList<EstanciaDto>> ListarAsync(CancellationToken ct = default)
    {
        var estancias = currentUserService.Rol == RolUsuario.Captador.ToString() && currentUserService.UsuarioId is { } captadorId
            ? await estanciaRepository.GetByCaptadorAsync(captadorId, ct)
            : await estanciaRepository.GetAllAsync(ct);

        return estancias.Select(ToDto).ToList();
    }

    public async Task<EstanciaDto> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var estancia = await estanciaRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Estancia), id);

        return ToDto(estancia);
    }

    public async Task<EstanciaDto> CrearAsync(CreateEstanciaDto dto, CancellationToken ct = default)
    {
        var captadorId = currentUserService.UsuarioId
            ?? throw new UnauthorizedException("Solo un captador autenticado puede registrar una estancia.");

        var estancia = new Estancia
        {
            CaptadorId = captadorId,
            Nombre = dto.Nombre,
            Propietario = dto.Propietario,
            Representante = dto.Representante,
            Telefono = dto.Telefono,
            Latitud = dto.Latitud,
            Longitud = dto.Longitud,
            Renspa = dto.Renspa,
            HectareasTotales = dto.HectareasTotales,
            Departamento = dto.Departamento,
            Provincia = dto.Provincia,
            Municipio = dto.Municipio,
            CreadoPorUsuarioId = captadorId,
            FechaCreacionLocal = dto.FechaCreacionLocal,
            EstadoSync = EstadoSync.Sincronizado
        };

        await estanciaRepository.AddAsync(estancia, ct);
        await estanciaRepository.SaveChangesAsync(ct);
        await auditoriaService.RegistrarAsync(AccionAuditoria.Creacion, "Estancia", estancia.Id, estancia.Nombre, ct);

        return ToDto(estancia);
    }

    public async Task<EstanciaDto> ActualizarAsync(Guid id, UpdateEstanciaDto dto, CancellationToken ct = default)
    {
        var estancia = await estanciaRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Estancia), id);

        estancia.Nombre = dto.Nombre;
        estancia.Propietario = dto.Propietario;
        estancia.Representante = dto.Representante;
        estancia.Telefono = dto.Telefono;
        estancia.Renspa = dto.Renspa;
        estancia.HectareasTotales = dto.HectareasTotales;
        estancia.Departamento = dto.Departamento;
        estancia.Provincia = dto.Provincia;
        estancia.Municipio = dto.Municipio;
        estancia.ModificadoPorUsuarioId = currentUserService.UsuarioId;
        estancia.FechaModificacion = DateTime.UtcNow;

        estanciaRepository.Update(estancia);
        await estanciaRepository.SaveChangesAsync(ct);
        await auditoriaService.RegistrarAsync(AccionAuditoria.Modificacion, "Estancia", estancia.Id, estancia.Nombre, ct);

        return ToDto(estancia);
    }

    public async Task EliminarAsync(Guid id, CancellationToken ct = default)
    {
        var estancia = await estanciaRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Estancia), id);

        estanciaRepository.Remove(estancia);
        await estanciaRepository.SaveChangesAsync(ct);
        await auditoriaService.RegistrarAsync(AccionAuditoria.Eliminacion, "Estancia", id, estancia.Nombre, ct);
    }

    private static EstanciaDto ToDto(Estancia e) => new(
        e.Id,
        e.Nombre,
        e.Propietario,
        e.Representante,
        e.Telefono,
        e.Latitud,
        e.Longitud,
        e.Renspa,
        e.HectareasTotales,
        e.Departamento,
        e.Provincia,
        e.Municipio,
        e.Captaciones?.Count ?? 0,
        e.EstadoSync.ToString());
}

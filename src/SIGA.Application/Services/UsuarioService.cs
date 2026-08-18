using SIGA.Application.Common;
using SIGA.Application.Common.Exceptions;
using SIGA.Application.DTOs.Usuarios;
using SIGA.Application.Interfaces;
using SIGA.Domain.Entities;
using SIGA.Domain.Enums;

namespace SIGA.Application.Services;

public class UsuarioService(
    IUsuarioRepository usuarioRepository,
    IPasswordHasher passwordHasher,
    IAuditoriaService auditoriaService) : IUsuarioService
{
    public async Task<IReadOnlyList<UsuarioDto>> ListarAsync(CancellationToken ct = default)
    {
        var usuarios = await usuarioRepository.GetAllAsync(ct);
        return usuarios.Select(ToDto).ToList();
    }

    public async Task<UsuarioDetalleDto> ObtenerDetalleAsync(Guid id, CancellationToken ct = default)
    {
        var usuario = await usuarioRepository.GetConDetalleAsync(id, ct)
            ?? throw new NotFoundException(nameof(Usuario), id);

        return new UsuarioDetalleDto(
            usuario.Id,
            usuario.Nombre,
            usuario.Email,
            usuario.Cargo,
            usuario.Rol.ToString(),
            usuario.Estado.ToString(),
            usuario.FechaCreacion,
            usuario.SectoresAsignados.Select(s => new SectorAsignadoDto(s.Id, s.NombreSector, s.Zona)).ToList(),
            usuario.Dispositivos.Select(d => new DispositivoDto(d.Id, d.IdentificadorDispositivo, d.UltimaSincronizacion, d.UbicacionActual, d.Estado.ToString())).ToList(),
            usuario.Permisos.Select(p => new PermisoUsuarioDto(p.Id, p.TipoPermiso.ToString())).ToList());
    }

    public async Task<UsuarioDto> CrearAsync(CreateUsuarioDto dto, CancellationToken ct = default)
    {
        if (await usuarioRepository.ExistsByEmailAsync(dto.Email, ct))
        {
            throw new ConflictException($"Ya existe un usuario con el correo '{dto.Email}'.");
        }

        var rol = EnumParser.Parse<RolUsuario>(dto.Rol, nameof(dto.Rol));

        Usuario usuario = rol == RolUsuario.Captador ? new Captador() : new Administrador();
        usuario.Nombre = dto.Nombre;
        usuario.Email = dto.Email;
        usuario.Cargo = dto.Cargo;
        usuario.PasswordHash = passwordHasher.Hash(dto.Password);
        usuario.Estado = EstadoUsuario.Pendiente;

        await usuarioRepository.AddAsync(usuario, ct);
        await usuarioRepository.SaveChangesAsync(ct);
        await auditoriaService.RegistrarAsync(AccionAuditoria.Creacion, "Usuario", usuario.Id, usuario.Email, ct);

        return ToDto(usuario);
    }

    public async Task<UsuarioDto> ActualizarAsync(Guid id, UpdateUsuarioDto dto, CancellationToken ct = default)
    {
        var usuario = await usuarioRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Usuario), id);

        usuario.Nombre = dto.Nombre;
        usuario.Cargo = dto.Cargo;
        usuario.Estado = EnumParser.Parse<EstadoUsuario>(dto.Estado, nameof(dto.Estado));

        usuarioRepository.Update(usuario);
        await usuarioRepository.SaveChangesAsync(ct);
        await auditoriaService.RegistrarAsync(AccionAuditoria.Modificacion, "Usuario", usuario.Id, usuario.Email, ct);

        return ToDto(usuario);
    }

    public async Task<SectorAsignadoDto> AsignarSectorAsync(Guid usuarioId, CreateSectorAsignadoDto dto, CancellationToken ct = default)
    {
        var usuario = await usuarioRepository.GetConDetalleAsync(usuarioId, ct)
            ?? throw new NotFoundException(nameof(Usuario), usuarioId);

        var sector = new SectorAsignado { UsuarioId = usuarioId, NombreSector = dto.NombreSector, Zona = dto.Zona };
        usuario.SectoresAsignados.Add(sector);

        usuarioRepository.Update(usuario);
        await usuarioRepository.SaveChangesAsync(ct);
        await auditoriaService.RegistrarAsync(AccionAuditoria.Modificacion, "Usuario.Sectores", usuarioId, dto.NombreSector, ct);

        return new SectorAsignadoDto(sector.Id, sector.NombreSector, sector.Zona);
    }

    public async Task QuitarSectorAsync(Guid usuarioId, Guid sectorId, CancellationToken ct = default)
    {
        var usuario = await usuarioRepository.GetConDetalleAsync(usuarioId, ct)
            ?? throw new NotFoundException(nameof(Usuario), usuarioId);

        var sector = usuario.SectoresAsignados.FirstOrDefault(s => s.Id == sectorId)
            ?? throw new NotFoundException(nameof(SectorAsignado), sectorId);

        usuario.SectoresAsignados.Remove(sector);
        usuarioRepository.Update(usuario);
        await usuarioRepository.SaveChangesAsync(ct);
        await auditoriaService.RegistrarAsync(AccionAuditoria.Modificacion, "Usuario.Sectores", usuarioId, $"Quitado: {sector.NombreSector}", ct);
    }

    public async Task RevocarDispositivoAsync(Guid usuarioId, Guid dispositivoId, CancellationToken ct = default)
    {
        var usuario = await usuarioRepository.GetConDetalleAsync(usuarioId, ct)
            ?? throw new NotFoundException(nameof(Usuario), usuarioId);

        var dispositivo = usuario.Dispositivos.FirstOrDefault(d => d.Id == dispositivoId)
            ?? throw new NotFoundException(nameof(Dispositivo), dispositivoId);

        dispositivo.Estado = EstadoDispositivo.Revocado;
        usuarioRepository.Update(usuario);
        await usuarioRepository.SaveChangesAsync(ct);
        await auditoriaService.RegistrarAsync(AccionAuditoria.Modificacion, "Usuario.Dispositivos", usuarioId, $"Revocado: {dispositivo.IdentificadorDispositivo}", ct);
    }

    public async Task<PermisoUsuarioDto> AsignarPermisoAsync(Guid usuarioId, AsignarPermisoDto dto, CancellationToken ct = default)
    {
        var usuario = await usuarioRepository.GetConDetalleAsync(usuarioId, ct)
            ?? throw new NotFoundException(nameof(Usuario), usuarioId);

        var tipoPermiso = EnumParser.Parse<TipoPermiso>(dto.TipoPermiso, nameof(dto.TipoPermiso));

        if (usuario.Permisos.Any(p => p.TipoPermiso == tipoPermiso))
        {
            throw new ConflictException($"El usuario ya tiene el permiso '{tipoPermiso}'.");
        }

        var permiso = new PermisoUsuario { UsuarioId = usuarioId, TipoPermiso = tipoPermiso };
        usuario.Permisos.Add(permiso);

        usuarioRepository.Update(usuario);
        await usuarioRepository.SaveChangesAsync(ct);
        await auditoriaService.RegistrarAsync(AccionAuditoria.Modificacion, "Usuario.Permisos", usuarioId, tipoPermiso.ToString(), ct);

        return new PermisoUsuarioDto(permiso.Id, permiso.TipoPermiso.ToString());
    }

    private static UsuarioDto ToDto(Usuario u) => new(
        u.Id, u.Nombre, u.Email, u.Cargo, u.Rol.ToString(), u.Estado.ToString(), u.FechaCreacion);
}

using Microsoft.EntityFrameworkCore;
using SIGA.Domain.Entities;

namespace SIGA.Infrastructure.Persistence;

public class SigaDbContext(DbContextOptions<SigaDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Estancia> Estancias => Set<Estancia>();
    public DbSet<CaptacionGanado> CaptacionesGanado => Set<CaptacionGanado>();
    public DbSet<DetalleLoteGanado> DetallesLoteGanado => Set<DetalleLoteGanado>();
    public DbSet<RegistroAlimentacion> RegistrosAlimentacion => Set<RegistroAlimentacion>();
    public DbSet<MovimientoGanado> MovimientosGanado => Set<MovimientoGanado>();
    public DbSet<RegistroSanitario> RegistrosSanitarios => Set<RegistroSanitario>();
    public DbSet<RegistroPesaje> RegistrosPesaje => Set<RegistroPesaje>();
    public DbSet<SectorAsignado> SectoresAsignados => Set<SectorAsignado>();
    public DbSet<Dispositivo> Dispositivos => Set<Dispositivo>();
    public DbSet<PermisoUsuario> PermisosUsuario => Set<PermisoUsuario>();
    public DbSet<LogAuditoria> LogsAuditoria => Set<LogAuditoria>();
    public DbSet<Departamento> Departamentos => Set<Departamento>();
    public DbSet<Provincia> Provincias => Set<Provincia>();
    public DbSet<Municipio> Municipios => Set<Municipio>();
    public DbSet<Raza> Razas => Set<Raza>();
    public DbSet<ProductoTratamiento> ProductosTratamiento => Set<ProductoTratamiento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SigaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
